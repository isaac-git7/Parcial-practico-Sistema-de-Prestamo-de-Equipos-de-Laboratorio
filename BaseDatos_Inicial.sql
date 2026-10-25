IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Categorias] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(50) NOT NULL,
    [Descripcion] nvarchar(200) NULL,
    CONSTRAINT [PK_Categorias] PRIMARY KEY ([Id])
);

CREATE TABLE [Usuarios] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(50) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [NombreCompleto] nvarchar(100) NOT NULL,
    [Rol] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
);

CREATE TABLE [Equipos] (
    [Id] int NOT NULL IDENTITY,
    [CodigoInventario] nvarchar(20) NOT NULL,
    [Nombre] nvarchar(100) NOT NULL,
    [Marca] nvarchar(50) NULL,
    [Modelo] nvarchar(50) NULL,
    [Serie] nvarchar(50) NULL,
    [Estado] nvarchar(20) NOT NULL,
    [CategoriaId] int NOT NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_Equipos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Equipos_Categorias_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [Categorias] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Prestamos] (
    [Id] int NOT NULL IDENTITY,
    [EquipoId] int NOT NULL,
    [UsuarioId] int NOT NULL,
    [FechaPrestamo] datetime2 NOT NULL,
    [FechaVencimiento] datetime2 NOT NULL,
    [FechaDevolucion] datetime2 NULL,
    [Estado] nvarchar(20) NOT NULL,
    [Observaciones] nvarchar(250) NULL,
    CONSTRAINT [PK_Prestamos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Prestamos_Equipos_EquipoId] FOREIGN KEY ([EquipoId]) REFERENCES [Equipos] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Prestamos_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Descripcion', N'Nombre') AND [object_id] = OBJECT_ID(N'[Categorias]'))
    SET IDENTITY_INSERT [Categorias] ON;
INSERT INTO [Categorias] ([Id], [Descripcion], [Nombre])
VALUES (1, N'Laptops y ultrabooks para prácticas de laboratorio', N'Cómputo Portátil'),
(2, N'Osciloscopios, fuentes de poder y generadores de señales', N'Instrumentación y Electrónica'),
(3, N'Kits Arduino, ESP32, Raspberry Pi y sensores', N'Microcontroladores e IoT'),
(4, N'Proyectores, cámaras y micrófonos para conferencias', N'Audiovisual');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Descripcion', N'Nombre') AND [object_id] = OBJECT_ID(N'[Categorias]'))
    SET IDENTITY_INSERT [Categorias] OFF;

CREATE UNIQUE INDEX [IX_Categorias_Nombre] ON [Categorias] ([Nombre]);

CREATE INDEX [IX_Equipos_CategoriaId] ON [Equipos] ([CategoriaId]);

CREATE UNIQUE INDEX [IX_Equipos_CodigoInventario] ON [Equipos] ([CodigoInventario]);

CREATE INDEX [IX_Prestamos_EquipoId] ON [Prestamos] ([EquipoId]);

CREATE INDEX [IX_Prestamos_UsuarioId] ON [Prestamos] ([UsuarioId]);

CREATE UNIQUE INDEX [IX_Usuarios_Username] ON [Usuarios] ([Username]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261025003542_InitialCreate', N'10.0.13');

COMMIT;
GO

