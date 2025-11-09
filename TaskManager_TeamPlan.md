# Task Management API - Team Plan & Setup

This document outlines how a team of three will build a .NET 8 Task Management API with JWT authentication, Entity Framework Core (SQLite), clean architecture (Controllers, Services, Models, Data), and controller tests.  

!!! The current state of the project described at the very bottom of this file ( see Current Project Status ).

## 1. Roles and Responsibilities

### Student A - Evgenii (DB Lead)
Responsible for Database & Models
- Owns: Models/, Data/TaskDbContext.cs, migrations, seeding, connection string
- Deliverables: User, Project, TaskItem (optional Comment) models; TaskDbContext with DbSets and relationships; seed sample data; SQLite wiring in Program.cs; first migration applied
- Contract: Model shapes stable; DbContext name = TaskDbContext; seeded users (e.g., alice Id=1, bob Id=2) with known password

### Student B - Vu Hoc (Head of Controllers & Services)
Responsible for Projects/Tasks/Comments controllers and services
- Owns: Controllers for Project/Task (optional Comment), Services for Project/Task, DTOs
- Rules: Controllers thin; business logic in Services; enforce ownership using IUserContext
- Unit tests: happy paths, 404 (non-owner), invalid cases

### Student C - Ayush (Security Boss)
Responsible for Auth (JWT) & Users
- Owns: AuthController (register/login), TokenService, JWT wiring in Program.cs, Swagger JWT
- Token claims: sub = user id, unique_name = username
- Definition of Done: protected endpoints require Bearer token; unauthorized yields 401

## 2. Repository & Branching

- ✅ Initial setup (one person):
  ```bash
  mkdir C:\task-mgr && cd C:\task-mgr
  dotnet new sln -n TaskManager
  dotnet new webapi -n TaskManager.Api --use-controllers
  dotnet new xunit -n TaskManager.Tests
  dotnet sln add .\TaskManager.Api\TaskManager.Api.csproj .\TaskManager.Tests\TaskManager.Tests.csproj
  git init && git add . && git commit -m "chore: solution bootstrap"
  ```

- Branches: 
  + main (protected),  
  + feat/db (A),  
  - feat/projects-tasks (B), 
  - feat/auth (C).  
  Optionally: PRs must build & pass tests.

## 3. Data Model (Owned by Student A)

### Models
- User: Id, Username, Email, PasswordHash, Projects, AssignedTasks, Comments?
- Project: Id, Name, Description, CreatedDate, UserId (owner), Tasks
- TaskItem: Id, Title, Description, Status (ToDo/InProgress/Done), DueDate, ProjectId, AssignedUserId, Comments?
- Comment (optional): Id, Text, CreatedDate, TaskItemId, UserId

### DbContext (TaskDbContext)
- DbSet<User>, DbSet<Project>, DbSet<TaskItem>, DbSet<Comment>
- HasData() seeding for users/projects/tasks
- SQLite connection string in appsettings.json: ConnectionStrings:Default = "Data Source=tasks.db"
- Migrations: `dotnet ef migrations add InitialCreate && dotnet ef database update`

## 4. Program.cs Wiring (Owned by A & C)

- EF Core: `AddDbContext<TaskDbContext>(UseSqlite(...))`
- CORS: `AddCors("frontend")` allowing any origin/headers/methods (dev)
- Authentication/Authorization: `AddAuthentication(JwtBearer)` + `AddAuthorization`
- Swagger with JWT security definition
- `MapControllers()`; optional root redirect to /swagger

## 5. Auth Layer (Owned by Student C)

- DTOs: RegisterDto(username,email,password), LoginDto(username,password), AuthResultDto(token, userId,username)
- TokenService: creates JWT with sub and unique_name claims
- AuthController: `/api/auth/register` and `/api/auth/login`; hashes passwords with BCrypt; returns token & user info
- Swagger: Authorize button with Bearer token

## 6. Controllers & Services (Owned by Student B)

### Abstraction for current user
- IUserContext { int UserId { get; } } and HttpUserContext reading claim "sub"
- Register IHttpContextAccessor & IUserContext in DI; tests can use FakeUserContext

### Controllers
- ProjectController (Authorize): CRUD; only owner can read/update/delete
- TaskController (Authorize): CRUD; UpdateStatus; Assign; only owner's projects' tasks accessible
- Optional CommentController: CRUD on task comments

### Services
- ProjectService/TaskService: implement queries with owner checks and map to DTOs

## 7. Endpoints (Contracts)

### Auth
- POST `/api/auth/register` { username, email, password } → { token, userId, username }
- POST `/api/auth/login` { username, password } → { token, userId, username }

### Projects
- GET `/api/project?search=`
- POST `/api/project` { name, description }
- GET `/api/project/{id}`
- PUT `/api/project/{id}` { name, description }
- DELETE `/api/project/{id}`

### Tasks
- GET `/api/task?projectId=&status=`
- POST `/api/task` { title, description, status, dueDate, projectId, assignedUserId }
- GET `/api/task/{id}`
- PUT `/api/task/{id}` { title, description, status, dueDate, projectId, assignedUserId }
- PATCH `/api/task/{id}/status` { status }
- PATCH `/api/task/{id}/assign` { assignedUserId }
- DELETE `/api/task/{id}`

## 8. Testing & Coverage

- Each student writes controller tests for their area (happy paths, 401/404/invalid)
- Use in-memory SQLite; FakeUserContext(UserId=1) for ownership
- Run tests: `dotnet test`
- Coverage (controllers-only):
  ```bash
  dotnet test .\TaskManager.Tests\TaskManager.Tests.csproj --collect "XPlat Code Coverage" --results-directory ".\TestResults"
  reportgenerator -reports:".\TestResults\**\coverage.cobertura.xml" -targetdir:".\TestReport.ControllersOnly" -reporttypes:Html -assemblyfilters:+TaskManager.Api -classfilters:+TaskManager.Api.Controllers.*
  ```

## 9. Work Process Notes (for the write-up)

- What worked: parallel work via clear ownership; IUserContext abstraction; seeds enabling quick manual tests
- Challenges: ownership checks across joins; JWT wiring and Swagger Bearer setup; migration conflicts
- Do differently: add paging/sorting early; add CI workflow to run tests on PR; add DTO annotations for validation
- Extras (optional): comments, notifications on status change/assignment, file URLs on tasks, sorting & filtering

## 10. Risks & Mitigations

- Schema churn → DB lead as single owner of migrations/seed; announce changes via PR
- Auth coupling → Controllers depend on IUserContext only; tests use FakeUserContext
- Permissions bugs → Centralize owner filtering in Services; add negative (non-owner) tests
- Merge conflicts → Branch per feature; small PRs; code reviews

## Current Project Status

✅ **Phase 1: Environment Setup and Dependencies**
- Installed Entity Framework Core packages:
  - Microsoft.EntityFrameworkCore.Sqlite
  - Microsoft.EntityFrameworkCore.Tools
  - Microsoft.EntityFrameworkCore.Design
- Configured SQLite connection string in appsettings.json

✅ **Phase 2: Model Implementation**
- Created User model with properties and navigation properties
- Created Project model with properties and navigation properties
- Created TaskItem model with properties, navigation properties, and TaskStatus enum
- Created Comment model with properties and navigation properties

🔄 **Next Steps**
- Implement TaskDbContext with DbSets and relationships
- Configure model relationships in OnModelCreating
- Add seed data for alice and bob users
- Register DbContext in DI container
- Generate and apply initial migration