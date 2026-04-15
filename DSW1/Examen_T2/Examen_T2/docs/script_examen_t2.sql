USE BDCURSOS2026API;
GO

CREATE OR ALTER PROCEDURE dbo.usp_listar_cursos_disponibles
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.id_baja,
        c.id_curso,
        c.nombre_curso,
        c.aforo,
        c.fecha_creacion
    FROM dbo.tb_cursos c
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.tb_cursos_baja cb
        WHERE cb.id_curso = c.id_curso
    )
    ORDER BY c.nombre_curso, c.id_curso;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_filtrar_cursos_por_iniciales
    @iniciales VARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.id_baja,
        c.id_curso,
        c.nombre_curso,
        c.aforo,
        c.fecha_creacion
    FROM dbo.tb_cursos c
    WHERE c.nombre_curso LIKE LTRIM(RTRIM(@iniciales)) + '%'
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.tb_cursos_baja cb
          WHERE cb.id_curso = c.id_curso
      )
    ORDER BY c.nombre_curso, c.id_curso;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_obtener_curso_por_id
    @id_curso INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        c.id_baja,
        c.id_curso,
        c.nombre_curso,
        c.aforo,
        c.fecha_creacion
    FROM dbo.tb_cursos c
    WHERE c.id_curso = @id_curso
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.tb_cursos_baja cb
          WHERE cb.id_curso = c.id_curso
      );
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_dar_baja_curso
    @id_curso INT,
    @cantidad_estudiantes INT,
    @exito BIT OUTPUT,
    @mensaje VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.tb_cursos WHERE id_curso = @id_curso)
        BEGIN
            SET @exito = 0;
            SET @mensaje = 'El curso indicado no existe.';
            RETURN;
        END;

        IF EXISTS (SELECT 1 FROM dbo.tb_cursos_baja WHERE id_curso = @id_curso)
        BEGIN
            SET @exito = 0;
            SET @mensaje = 'El curso ya fue dado de baja.';
            RETURN;
        END;

        BEGIN TRANSACTION;

        INSERT INTO dbo.tb_cursos_baja (id_curso)
        VALUES (@id_curso);

        IF @cantidad_estudiantes > 0
        BEGIN
            INSERT INTO dbo.tb_cursos_archivados (id_curso, cantidad_estudiantes)
            VALUES (@id_curso, @cantidad_estudiantes);
        END;

        COMMIT TRANSACTION;

        SET @exito = 1;
        SET @mensaje =
            CASE
                WHEN @cantidad_estudiantes > 0
                    THEN 'Curso dado de baja y archivado correctamente.'
                ELSE 'Curso dado de baja correctamente.'
            END;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        SET @exito = 0;
        SET @mensaje = 'Error al procesar la baja: ' + ERROR_MESSAGE();
    END CATCH;
END;
GO
