# Demo CRUD Productos (.NET 8 + HTML/JS) con Orquestación Copilot

Aplicación ejemplo con API Web minimal en .NET 8, datos en memoria, Swagger y un frontend HTML/JS simple. CORS está habilitado para `http://localhost:5500`.

**✨ Novedad**: Sistema de **agentes Copilot** para validación automática de PRs con documentación HTML consultable, revisión de calidad y análisis de cobertura de tests.

## Características

- 🚀 API REST minimal con .NET 8
- 📊 Swagger UI habilitado
- 💾 Datos en memoria (mock)
- 🌐 Frontend HTML/JS vanilla
- 🔒 CORS configurado
- ✅ Tests unitarios xUnit
- 🤖 **Agentes Copilot para PRs**:
  - Generador de documentación HTML
  - Validador de calidad de código
  - Analizador de cobertura de tests

## Requisitos
- .NET 8 SDK
- Navegador moderno
- (Opcional) `npx serve` u otro servidor estático para el frontend

## Compilar el proyecto

Para compilar todo el proyecto (API + Tests):
```bash
dotnet build copilot-agentic-workflow-demo-v3.sln
```

Para compilar solo la API:
```bash
dotnet build copilot-agentic-workflow-demo-v3.csproj
```

## Ejecutar API
```bash
dotnet run
```
La API queda en:
- HTTPS: https://localhost:5001
- Swagger: https://localhost:5001/swagger

## Servir el frontend (HTML/JS)
En otra terminal:
```bash
cd frontend
npx serve -l 5500
```
Abrir en el navegador: http://localhost:5500

## Endpoints principales
- `GET /api/productos` — Listar
- `GET /api/productos/{id}` — Obtener uno
- `POST /api/productos` — Crear
- `PUT /api/productos/{id}` — Actualizar
- `DELETE /api/productos/{id}` — Eliminar

## Tests unitarios

El proyecto incluye tests xUnit en `copilot-agentic-workflow-demo-v3.Tests/`:

```bash
dotnet test copilot-agentic-workflow-demo-v3.sln
```

Para ejecutar con cobertura de código:
```bash
dotnet test copilot-agentic-workflow-demo-v3.sln --collect:"XPlat Code Coverage"
```

**Tests disponibles**:
- ✅ 9 tests para CRUD completo de Producto
- ✅ Happy path, validaciones, errores esperados
- ✅ Integración con `WebApplicationFactory`

## Orquestación de Agentes Copilot

> Nota: este repositorio incluye PRs de ejemplo para disparar el workflow y ver los reportes en **Actions → Artifacts**.

Este proyecto implementa un **workflow automático** que se ejecuta en:
- ✅ Pull Requests (abiertos, actualizados)
- ✅ Push a `main`

### Agentes especializados

1. **doc-generator** ([ver definición](.github/agents/doc-generator.agent.md))
   - Genera reporte HTML consultable con cambios de PR/commit
   - Incluye diffs, estadísticas, resultados de tests y cobertura
   - Output: HTML autónomo con estilos inline

2. **code-quality** ([ver definición](.github/agents/code-quality.agent.md))
   - Revisa mejores prácticas .NET (null-safety, naming, async patterns)
   - Detecta violaciones SOLID y problemas de performance
   - Output: Markdown con issues y sugerencias

3. **test-validator** ([ver definición](.github/agents/test-validator.agent.md))
   - Analiza cobertura de tests existentes
   - Identifica gaps y sugiere tests nuevos
   - Output: Markdown con tests sugeridos

### Flujo de validación

```
PR/Push → Build & Test → [code-quality + test-validator] → doc-generator → HTML Report
                                                              ↓
                                                    Artifact + PR Comment
```

Ver detalles completos en [.github/agents/README.md](.github/agents/README.md)

### Usar el workflow

1. **Crear Pull Request**:
   ```bash
   git checkout -b feature/nuevo-endpoint
   # ... hacer cambios ...
   git commit -m "feat: añadir endpoint de categorías"
   git push origin feature/nuevo-endpoint
   ```

2. **Workflow automático**:
   - Se ejecuta automáticamente al abrir/actualizar PR
   - Genera reporte HTML en artifacts
   - Comenta en el PR con resumen y link al reporte

3. **Descargar reporte**:
   - Ve a **Actions** → workflow run → **Artifacts** → `html-documentation`
   - Descarga y abre el HTML en navegador

## Estructura del proyecto

```
copilot-agentic-workflow-demo-v3/
├── Program.cs                       # API minimal .NET 8
├── copilot-agentic-workflow-demo-v3.csproj
├── frontend/
│   └── index.html                   # UI simple con fetch
├── copilot-agentic-workflow-demo-v3.Tests/
│   ├── ProductoEndpointsTests.cs    # Tests xUnit
│   └── copilot-agentic-workflow-demo-v3.Tests.csproj
└── .github/
    ├── copilot-instructions.md      # Instrucciones globales Copilot
    ├── agents/
    │   ├── doc-generator.agent.md   # Agente documentación
    │   ├── code-quality.agent.md    # Agente calidad
    │   ├── test-validator.agent.md  # Agente tests
    │   └── README.md                # Guía de agentes
    └── workflows/
        └── copilot-pr-validation.yml # Workflow CI/CD
```

## Notas
- Datos en memoria (se reinician al relanzar).
- Validaciones mínimas: nombre requerido, precio y stock no negativos.
- Si el frontend no carga por CORS, revisa que se use `http://localhost:5500` como origen.
- Los agentes Copilot generan reportes en español por defecto (configurable en cada agente).

## Próximos pasos

- [x] ~~Ajustar dependencias xUnit para ejecución completa~~ (Completado)
- [ ] Agregar más tests de integración
- [ ] Implementar persistencia (opcional)
- [ ] Extender agentes con análisis de seguridad
- [ ] Integrar GitHub Pages para reportes HTML permanentes

## Recursos

- [Guía de Agentes Copilot](.github/agents/README.md)
- [Workflow de validación](.github/workflows/copilot-pr-validation.yml)
- [Documentación .NET 8](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-8)
- [xUnit.net](https://xunit.net/)

---

**Versión**: 1.0.0  
**Stack**: .NET 8, xUnit, GitHub Actions, Copilot Agents  
**Licencia**: MIT
