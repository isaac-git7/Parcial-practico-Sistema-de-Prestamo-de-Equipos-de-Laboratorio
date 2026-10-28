# Sistema de Gestión de Préstamos de Laboratorio

Sistema web distribuido desarrollado en **ASP.NET Core (.NET 10)** con arquitectura desacoplada: **Web API RESTful** con **Entity Framework Core / SQL Server** en el backend y cliente web **ASP.NET Core MVC** en el frontend.

---

## 🛠️ Arquitectura y Tecnologías

* **Backend (`LaboratorioAPI`):**
  * ASP.NET Core Web API con controladores RESTful.
  * **Entity Framework Core**: Code First / DbContext con SQL Server.
  * **AutoMapper (v16.x)**: Transformación bidireccional entre entidades de dominio y DTOs especializados.
  * **Control de Concurrencia Optimista**: Implementado mediante tokens de fila (`[Timestamp] byte[] RowVersion`) serializados en Base64 para prevenir sobreescrituras en actualizaciones concurrentes (`409 Conflict`).
  * **Documentación interactiva**: Swagger / OpenAPI integrado.

* **Frontend (`LaboratorioMVC`):**
  * ASP.NET Core MVC desacoplado consumiendo la API mediante `HttpClient` tipado e inyección de dependencias (`IEquipoService`, `ICategoriaService`, `IReporteService`).
  * Paginación en servidor/controlador a razón de **10 elementos por página**.
  * Filtros combinados en tiempo real por búsqueda de texto, categoría y estado operativo.
  * Módulo de Reportes: Tablero operativo (KPIs, mora, top más solicitados) y Reporte 4 (Historial consolidado por usuario).

* **Pruebas y Validación:**
  * Postman Collection Runner con pruebas funcionales automatizadas (`pm.test`).

---

## 🚀 Requisitos Previos y Ejecución

1. **Base de Datos (SQL Server):**
   * Configurar la cadena de conexión en `LaboratorioAPI/appsettings.json`:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=LaboratorioPrestamosDb;Trusted_Connection=True;TrustServerCertificate=True;"
     }
     ```
   * Aplicar migraciones desde la terminal del Administrador de Paquetes:
     ```bash
     Update-Database -Project LaboratorioAPI
     ```

2. **Ejecución Local:**
   * Establecer inicio múltiple en la solución (`LaboratorioAPI` y `LaboratorioMVC`) o ejecutar por terminal:
     ```bash
     dotnet run --project LaboratorioAPI
     dotnet run --project LaboratorioMVC
     ```

---

## 🧪 Pruebas Automatizadas en Postman (Fase 4 - Persona A)

La colección oficial se encuentra en `Postman/Laboratorio_Fase4_PersonaA.postman_collection.json`. Los casos automatizados cubren:

| Caso | Endpoint | Método | Código Esperado | Regla / Validación Evaluada |
| :--- | :--- | :---: | :---: | :--- |
| **Caso 4** | `/api/Equipos` | `GET` | `200 OK` | Filtros combinados simultáneos (texto, categoría y estado). |
| **Caso 6** | `/api/Prestamos` | `POST` | `400 Bad Request` | Rechazo de solicitud con fecha límite anterior al momento actual. |
| **Caso 7** | `/api/Prestamos` | `POST` | `400 Bad Request` | Validación de política institucional de préstamo máximo de 7 días. |
| **Auxiliar** | `/api/Prestamos` | `POST` | `201 Created` | Creación exitosa de préstamo que conmuta el equipo a estado *Prestado*. |
| **Caso 8** | `/api/Prestamos` | `POST` | `409 Conflict` | Conflicto de disponibilidad al intentar prestar un equipo ya ocupado. |
| **Caso 9** | `/api/Prestamos/{id}/devolver` | `POST` | `200 OK` | Cierre de préstamo y restauración automática del equipo a *Disponible*. |
| **Caso 12**| `/api/Equipos/{id}` | `PUT` | `409 Conflict` | Detección de colisión de concurrencia optimista (`RowVersion` desactualizado). |
