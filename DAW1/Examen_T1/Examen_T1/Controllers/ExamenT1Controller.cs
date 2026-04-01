using Examen_T1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Examen_T1.Controllers
{
    public class ExamenT1Controller : Controller
    {
        private readonly string _cn;

        public ExamenT1Controller(IConfiguration configuration)
        {
            _cn = configuration.GetConnectionString("cnBDVENTAS2025") ?? string.Empty;
        }

        // Pregunta 1 - Consulta de ventas por anio con paginacion de 10 filas.
        public IActionResult VentasPorAnio(int? year, int page = 1)
        {
            int anioConsulta = year ?? DateTime.Now.Year;
            List<VentasAnio> ventas = ListarVentasAnio(anioConsulta);

            const int pageSize = 10;
            int totalRegistros = ventas.Count;
            int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            page = Math.Max(1, page);
            if (totalPaginas > 0)
            {
                page = Math.Min(page, totalPaginas);
            }

            List<VentasAnio> ventasPagina = ventas
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Year = anioConsulta;
            ViewBag.Page = page;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;

            return View(ventasPagina);
        }

        // Pregunta 2 - Vista de mantenimiento con formulario + parcial.
        [HttpGet]
        public IActionResult Agregar()
        {
            CargarCategorias();
            ViewBag.Mensaje = TempData["Mensaje"]?.ToString();
            return View(new Articulo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Agregar(Articulo articulo)
        {
            if (!ModelState.IsValid)
            {
                CargarCategorias();
                ViewBag.Mensaje = "Revise los datos ingresados.";
                return View(articulo);
            }

            string mensaje = InsertarArticulo(articulo);
            TempData["Mensaje"] = mensaje;
            return RedirectToAction(nameof(Agregar));
        }

        private List<Categoria> ListarCategorias()
        {
            List<Categoria> categorias = new();

            using SqlConnection connection = new(_cn);
            using SqlCommand command = new("usp_listar_categorias", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                categorias.Add(new Categoria
                {
                    CodCat = Convert.ToInt32(reader["cod_cat"]),
                    NomCat = Convert.ToString(reader["nom_cat"]) ?? string.Empty
                });
            }

            return categorias;
        }

        private List<Articulo> ListarArticulos()
        {
            List<Articulo> articulos = new();

            using SqlConnection connection = new(_cn);
            using SqlCommand command = new("usp_listar_articulos", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                articulos.Add(new Articulo
                {
                    CodArt = Convert.ToString(reader["cod_art"]) ?? string.Empty,
                    NomArt = Convert.ToString(reader["nom_art"]) ?? string.Empty,
                    UniMed = Convert.ToString(reader["uni_med"]) ?? string.Empty,
                    PreArt = Convert.ToDecimal(reader["pre_art"]),
                    StkArt = Convert.ToInt32(reader["stk_art"]),
                    CodCat = Convert.ToInt32(reader["cod_cat"]),
                    NomCat = Convert.ToString(reader["nom_cat"]) ?? string.Empty
                });
            }

            return articulos;
        }

        private string InsertarArticulo(Articulo articulo)
        {
            try
            {
                using SqlConnection connection = new(_cn);
                using SqlCommand command = new("usp_agregar_articulo", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@cod_art", articulo.CodArt.Trim());
                command.Parameters.AddWithValue("@nom_art", articulo.NomArt.Trim());
                command.Parameters.AddWithValue("@uni_med", articulo.UniMed.Trim());
                command.Parameters.AddWithValue("@pre_art", articulo.PreArt);
                command.Parameters.AddWithValue("@stk_art", articulo.StkArt);
                command.Parameters.AddWithValue("@cod_cat", articulo.CodCat);

                SqlParameter pMensaje = new("@mensaje", SqlDbType.VarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(pMensaje);

                connection.Open();
                command.ExecuteNonQuery();

                return Convert.ToString(pMensaje.Value) ?? "Proceso ejecutado.";
            }
            catch (Exception ex)
            {
                return $"Error al insertar: {ex.Message}";
            }
        }

        private List<VentasAnio> ListarVentasAnio(int year)
        {
            List<VentasAnio> ventas = new();

            using SqlConnection connection = new(_cn);
            using SqlCommand command = new("usp_ventas_por_anio", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@year", year);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ventas.Add(new VentasAnio
                {
                    NumVta = Convert.ToString(reader["num_vta"]) ?? string.Empty,
                    FecVta = Convert.ToDateTime(reader["fec_vta"]),
                    NomCli = Convert.ToString(reader["nom_cli"]) ?? string.Empty,
                    NomVen = Convert.ToString(reader["nom_ven"]) ?? string.Empty,
                    TotVta = Convert.ToDecimal(reader["tot_vta"])
                });
            }

            return ventas;
        }

        private void CargarCategorias()
        {
            List<Categoria> categorias = ListarCategorias();
            ViewBag.Categorias = new SelectList(categorias, "CodCat", "NomCat");
            ViewBag.Articulos = ListarArticulos();
        }
    }
}
