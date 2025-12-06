# Task Management API - Team Plan & Setup

This document outlines how a team of three will build a .NET 8 Task Management API with JWT authentication, Entity Framework Core (SQLite), clean architecture (Controllers, Services, Models, Data), and controller tests.  

💡 The current state of the project described at the very bottom of this file ( see Current Project Status ).

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
- Installed authentication packages:
  - BCrypt.Net-Next
  - Microsoft.AspNetCore.Authentication.JwtBearer
  - System.IdentityModel.Tokens.Jwt
  - Microsoft.OpenApi
- Configured SQLite connection string in appsettings.json

✅ **Phase 2: Model Implementation (Student A - Evgenii)**
- Created User model with properties and navigation properties
- Created Project model with properties and navigation properties
- Created TaskItem model with properties, navigation properties, and TaskStatus enum
- Created Comment model with properties and navigation properties

✅ **Phase 3: DbContext Implementation (Student A - Evgenii)**
- Created TaskDbContext with DbSets for all models
- Configured model relationships in OnModelCreating
- Added seed data for alice and bob users, projects, and tasks
- Registered DbContext in DI container in Program.cs

✅ **Phase 4: Database Migration Application (Student A - Evgenii)**
- Created and applied initial database migration
- Database file created with all tables
- Seed data successfully inserted into database

✅ **Phase 5: Authentication Implementation (Student C - Ayush)**
- Implemented JWT authentication end-to-end
- Created AuthController with Register and Login endpoints
- Added BCrypt password hashing & verification
- Implemented TokenService issuing JWT with claims (sub = user id, unique_name = username)
- Created DTOs for Register, Login, and AuthResult
- Added JWT authentication + Authorization in Program.cs
- Added Swagger with Bearer security (Authorize button)
- Enabled CORS policy (frontend)
- Registered IUserContext + HttpUserContext to read current user from JWT
- OpenAPI renders correctly at /swagger with Bearer token authorization capability

✅ **Phase 6: Controllers, Services & DTOs Implementation (Student B - Vu Hoc)**

**Data Transfer Objects:**
- ProjectDtos: CreateProjectDto, UpdateProjectDto, ProjectDto (with TaskCount)
- TaskDtos: CreateTaskDto, UpdateTaskDto, UpdateTaskStatusDto, AssignTaskDto, TaskDto
- CommentDtos: CreateCommentDto, UpdateCommentDto, CommentDto

**Services (All with Ownership Enforcement):**
- ProjectService: GetProjectsAsync, GetProjectByIdAsync, CreateProjectAsync, UpdateProjectAsync, DeleteProjectAsync
- TaskService: GetTasksAsync, GetTaskByIdAsync, CreateTaskAsync, UpdateTaskAsync, UpdateTaskStatusAsync, AssignTaskAsync, DeleteTaskAsync
- CommentService: GetCommentsByTaskAsync, GetCommentByIdAsync, CreateCommentAsync, UpdateCommentAsync, DeleteCommentAsync

**Controllers (All Protected with [Authorize]):**
- ProjectController: Full CRUD with search filtering
- TaskController: Full CRUD + status updates + task assignment
- CommentController: Full CRUD on task comments

**Unit Tests:**
- ProjectControllerTests: Happy paths, ownership verification, search functionality
- TaskControllerTests: CRUD operations, status updates, assignment, ownership, filtering
- CommentControllerTests: CRUD operations, ownership verification
- Test Infrastructure: FakeUserContext and TestDbContextFactory for isolated testing

## Project Setup & Execution

### Initial Setup
```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build
```

### Environment Configuration
```bash
# Initialize user secrets (one-time)
dotnet user-secrets init

# Set JWT configuration (required for local development)
dotnet user-secrets set "Jwt:Key" "your-secret-key-min-32-characters-long"
dotnet user-secrets set "Jwt:Issuer" "TaskApi"
dotnet user-secrets set "Jwt:Audience" "TaskApiClients"
```

Alternatively, configure in `appsettings.Development.json`:
```json
{
  "Jwt": {
    "Key": "your-secret-key-min-32-characters-long",
    "Issuer": "TaskApi",
    "Audience": "TaskApiClients"
  }
}
```

### Database Setup
```bash
# Apply database migrations (if needed)
dotnet ef database update

# Run application
dotnet run
```

### Running Tests
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## API Usage

### Authentication Flow
1. Register: `POST /api/auth/register` with username, email, password
2. Login: `POST /api/auth/login` with username, password
3. Receive JWT token in response
4. Use Bearer token in Authorization header for all subsequent requests

### Example Request
```bash
curl -X GET "https://localhost:5001/api/project" \
  -H "Authorization: Bearer <your-jwt-token>"
```

Access Swagger UI at: `http://localhost:<port>/swagger`

## Security Features

✅ **JWT Authentication**: All endpoints require valid Bearer token  
✅ **Ownership Enforcement**: Users can only access/modify their own resources  
✅ **Project Ownership Verification**: Tasks are only accessible in owned projects  
✅ **Comment Ownership**: Users can only update/delete their own comments  
✅ **Input Validation**: DTOs enforce field constraints  
✅ **Password Security**: BCrypt hashing with salt  
✅ **CORS Policy**: Configured for frontend development  

## Key Implementation Details

### Ownership Enforcement Strategy
- Implemented at service layer using IUserContext
- Projects: `UserId == currentUserId` filter
- Tasks: `t.Project.UserId == userId` verification
- Comments: Task ownership + comment owner checks

### Test Infrastructure
- In-memory SQLite database for isolation
- FakeUserContext for simulating authenticated users
- TestDbContextFactory with seeded test data (alice: ID=1, bob: ID=2)
- Comprehensive test coverage for happy paths and negative cases

### Response Codes
- **200 OK**: Successful retrieval/update
- **201 Created**: Successful creation
- **204 No Content**: Successful deletion
- **400 Bad Request**: Invalid input or validation failure
- **401 Unauthorized**: Missing or invalid JWT token
- **403 Forbidden**: Insufficient permissions (non-owner access)
- **404 Not Found**: Resource not found or not owned by user

## Project Structure
```
TaskManagementAPI/
├── Controllers/
│   ├── AuthController.cs
│   ├── ProjectController.cs
│   ├── TaskController.cs
│   └── CommentController.cs
├── Services/
│   ├── IProjectService.cs / ProjectService.cs
│   ├── ITaskService.cs / TaskService.cs
│   ├── ICommentService.cs / CommentService.cs
│   ├── TokenService.cs
│   ├── IUserContext.cs / UserContext.cs
│   └── HttpUserContext.cs
├── Models/
│   ├── User.cs
│   ├── Project.cs
│   ├── TaskItem.cs
│   └── Comment.cs
├── DTOs/
│   ├── AuthDtos.cs
│   ├── ProjectDtos.cs
│   ├── TaskDtos.cs
│   └── CommentDtos.cs
├── Data/
│   └── TaskDbContext.cs
├── Migrations/
└── TaskManagementAPI.Tests/
    ├── Controllers/
    │   ├── ProjectControllerTests.cs
    │   ├── TaskControllerTests.cs
    │   └── CommentControllerTests.cs
    └── Helpers/
        ├── FakeUserContext.cs
        └── TestDbContextFactory.cs
```

## Work Process & Lessons Learned

**What Worked Well:**
- Parallel development via clear role-based ownership (A, B, C)
- IUserContext abstraction enabled clean separation of concerns
- Seed data facilitated quick manual testing
- Service layer pattern enabled comprehensive ownership enforcement

**Challenges & Solutions:**
- Ownership verification across table joins → Service layer filtering with relationship verification
- JWT wiring complexity → Centralized in Program.cs with clear configuration
- Swagger Bearer setup → Consistent pattern applied across all protected endpoints
- Test isolation → In-memory database with FakeUserContext implementation

**Recommendations for Future Development:**
- Implement paging and sorting from the start
- Add CI/CD workflow to run tests on PR
- Add data annotations for advanced validation (email format, phone numbers, etc.)
- Consider adding notification system for task status changes and assignments
- Implement audit logging for security-sensitive operations
- Add rate limiting and API key management

## Development Notes

**Database:**
- SQLite for development (easily switchable to SQL Server/PostgreSQL via connection string)
- Migrations tracked in source control
- Seed data ensures consistent test environment

**Debugging:**
- Enable debug logging in `appsettings.Development.json` for detailed error information
- Use Swagger UI to test endpoints interactively
- Review JWT tokens at [jwt.io](https://jwt.io) to verify claims