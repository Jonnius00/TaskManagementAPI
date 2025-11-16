# Task Management API - Implementation Report

## Overview
This report documents the implementation of Controllers, Services, and DTOs for the Task Management API, as assigned to Student B (Vu Hoc). The implementation follows clean architecture principles with thin controllers, business logic in services, and ownership enforcement using IUserContext.

## Components Implemented

### 1. Data Transfer Objects (DTOs)

#### Project DTOs (`DTOs/ProjectDtos.cs`)
- **CreateProjectDto**: Used for creating new projects
  - Name (required, max 100 chars)
  - Description (optional, max 500 chars)
  
- **UpdateProjectDto**: Used for updating existing projects
  - Name (required, max 100 chars)
  - Description (optional, max 500 chars)
  
- **ProjectDto**: Response DTO containing project information
  - Id, Name, Description, CreatedDate
  - UserId (owner), OwnerUsername
  - TaskCount (number of tasks in the project)

#### Task DTOs (`DTOs/TaskDtos.cs`)
- **CreateTaskDto**: Used for creating new tasks
  - Title (required, max 200 chars)
  - Description (optional, max 1000 chars)
  - Status (TaskStatus enum: ToDo, InProgress, Done)
  - DueDate
  - ProjectId (required)
  - AssignedUserId (optional)
  
- **UpdateTaskDto**: Used for updating existing tasks
  - Same fields as CreateTaskDto
  
- **UpdateTaskStatusDto**: Used for updating task status only
  - Status (TaskStatus enum)
  
- **AssignTaskDto**: Used for assigning tasks to users
  - AssignedUserId (optional, null to unassign)
  
- **TaskDto**: Response DTO containing task information
  - Id, Title, Description, Status, DueDate
  - ProjectId, ProjectName
  - AssignedUserId, AssignedUsername
  - CommentCount

#### Comment DTOs (`DTOs/CommentDtos.cs`)
- **CreateCommentDto**: Used for creating new comments
  - Text (required, max 1000 chars)
  - TaskItemId (required)
  
- **UpdateCommentDto**: Used for updating existing comments
  - Text (required, max 1000 chars)
  
- **CommentDto**: Response DTO containing comment information
  - Id, Text, CreatedDate
  - TaskItemId, TaskTitle
  - UserId, Username

### 2. Services

#### ProjectService (`Services/ProjectService.cs`)
Implements `IProjectService` interface with the following methods:

- **GetProjectsAsync**: Retrieves all projects owned by the user, with optional search filtering
- **GetProjectByIdAsync**: Retrieves a specific project by ID (only if owned by user)
- **CreateProjectAsync**: Creates a new project for the current user
- **UpdateProjectAsync**: Updates an existing project (only if owned by user)
- **DeleteProjectAsync**: Deletes a project (only if owned by user)

**Ownership Enforcement**: All methods filter by `UserId` to ensure users can only access their own projects.

#### TaskService (`Services/TaskService.cs`)
Implements `ITaskService` interface with the following methods:

- **GetTasksAsync**: Retrieves all tasks from user's projects, with optional filtering by projectId and status
- **GetTaskByIdAsync**: Retrieves a specific task (only if it belongs to user's projects)
- **CreateTaskAsync**: Creates a new task in one of the user's projects
- **UpdateTaskAsync**: Updates an existing task (only if it belongs to user's projects)
- **UpdateTaskStatusAsync**: Updates only the status of a task
- **AssignTaskAsync**: Assigns or unassigns a task to/from a user
- **DeleteTaskAsync**: Deletes a task (only if it belongs to user's projects)

**Ownership Enforcement**: All methods verify that tasks belong to projects owned by the user through `t.Project.UserId == userId` checks.

#### CommentService (`Services/CommentService.cs`)
Implements `ICommentService` interface with the following methods:

- **GetCommentsByTaskAsync**: Retrieves all comments for a task (only if task belongs to user's projects)
- **GetCommentByIdAsync**: Retrieves a specific comment (only if it belongs to a task in user's projects)
- **CreateCommentAsync**: Creates a new comment on a task (only if task belongs to user's projects)
- **UpdateCommentAsync**: Updates a comment (only if comment owner and task belongs to user's projects)
- **DeleteCommentAsync**: Deletes a comment (only if comment owner and task belongs to user's projects)

**Ownership Enforcement**: 
- Task ownership: Verifies task belongs to user's projects
- Comment ownership: Only comment owner can update/delete their comments

### 3. Controllers

#### ProjectController (`Controllers/ProjectController.cs`)
Protected with `[Authorize]` attribute. All endpoints require JWT authentication.

**Endpoints:**
- `GET /api/project` - Get all user's projects (optional `?search=` query parameter)
- `GET /api/project/{id}` - Get specific project by ID
- `POST /api/project` - Create new project
- `PUT /api/project/{id}` - Update existing project
- `DELETE /api/project/{id}` - Delete project

**Response Codes:**
- 200 OK: Successful retrieval/update
- 201 Created: Successful creation
- 204 No Content: Successful deletion
- 400 Bad Request: Invalid input data
- 404 Not Found: Project not found or not owned by user

#### TaskController (`Controllers/TaskController.cs`)
Protected with `[Authorize]` attribute. All endpoints require JWT authentication.

**Endpoints:**
- `GET /api/task` - Get all tasks from user's projects (optional `?projectId=` and `?status=` query parameters)
- `GET /api/task/{id}` - Get specific task by ID
- `POST /api/task` - Create new task
- `PUT /api/task/{id}` - Update existing task
- `PATCH /api/task/{id}/status` - Update task status only
- `PATCH /api/task/{id}/assign` - Assign/unassign task to user
- `DELETE /api/task/{id}` - Delete task

**Response Codes:**
- 200 OK: Successful retrieval/update
- 201 Created: Successful creation
- 204 No Content: Successful deletion
- 400 Bad Request: Invalid input data or invalid user/project reference
- 404 Not Found: Task not found or not accessible

#### CommentController (`Controllers/CommentController.cs`)
Protected with `[Authorize]` attribute. All endpoints require JWT authentication.

**Endpoints:**
- `GET /api/comment/task/{taskId}` - Get all comments for a task
- `GET /api/comment/{id}` - Get specific comment by ID
- `POST /api/comment` - Create new comment
- `PUT /api/comment/{id}` - Update existing comment
- `DELETE /api/comment/{id}` - Delete comment

**Response Codes:**
- 200 OK: Successful retrieval/update
- 201 Created: Successful creation
- 204 No Content: Successful deletion
- 400 Bad Request: Invalid input data
- 403 Forbidden: Attempting to update/delete comment not owned by user
- 404 Not Found: Comment/task not found or not accessible

### 4. Dependency Injection Registration

All services are registered in `Program.cs`:
```csharp
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ICommentService, CommentService>();
```

### 5. Unit Tests

A test project (`TaskManagementAPI.Tests`) has been created with:

#### Test Infrastructure
- **FakeUserContext**: Mock implementation of IUserContext for testing
- **TestDbContextFactory**: Factory for creating in-memory database contexts with seeded test data

#### Test Files
- **ProjectControllerTests.cs**: Tests for ProjectController
  - Happy path tests (create, read, update, delete)
  - Ownership tests (404 when accessing non-owned projects)
  - Search functionality tests
  
- **TaskControllerTests.cs**: Tests for TaskController
  - Happy path tests (CRUD operations)
  - Status update tests
  - Task assignment tests
  - Ownership tests (404 when accessing non-owned tasks)
  - Filtering tests (by projectId and status)
  
- **CommentControllerTests.cs**: Tests for CommentController
  - Happy path tests (CRUD operations)
  - Ownership tests (404 when accessing non-owned comments)
  - Comment owner verification tests

**Note**: Tests use in-memory SQLite database and FakeUserContext to isolate controller logic.

## Architecture Decisions

### 1. Thin Controllers
Controllers are kept thin and delegate all business logic to services. Controllers only handle:
- HTTP request/response mapping
- Model validation
- Status code determination
- Exception handling and conversion to appropriate HTTP responses

### 2. Service Layer Pattern
All business logic resides in services:
- Database queries and filtering
- Ownership verification
- Data transformation (Model to DTO)
- Business rule enforcement

### 3. Ownership Enforcement
Ownership is enforced at the service layer using `IUserContext`:
- Projects: Users can only access projects where `UserId == currentUserId`
- Tasks: Users can only access tasks from projects they own
- Comments: Users can only modify comments they created, but can view comments on tasks in their projects

### 4. DTO Pattern
DTOs are used to:
- Separate API contracts from internal models
- Prevent over-posting attacks
- Control what data is exposed to clients
- Include computed/aggregated data (e.g., TaskCount, CommentCount)

## Security Features

1. **JWT Authentication**: All endpoints require valid JWT token
2. **Ownership Checks**: Users can only access/modify their own resources
3. **Input Validation**: Data annotations on DTOs ensure valid input
4. **Project Ownership Verification**: Tasks can only be created/accessed in user's own projects
5. **Comment Ownership**: Users can only update/delete their own comments

## API Endpoints Summary

### Projects
- `GET /api/project` - List user's projects
- `GET /api/project/{id}` - Get project details
- `POST /api/project` - Create project
- `PUT /api/project/{id}` - Update project
- `DELETE /api/project/{id}` - Delete project

### Tasks
- `GET /api/task` - List tasks (filterable by projectId and status)
- `GET /api/task/{id}` - Get task details
- `POST /api/task` - Create task
- `PUT /api/task/{id}` - Update task
- `PATCH /api/task/{id}/status` - Update task status
- `PATCH /api/task/{id}/assign` - Assign task to user
- `DELETE /api/task/{id}` - Delete task

### Comments
- `GET /api/comment/task/{taskId}` - List comments for a task
- `GET /api/comment/{id}` - Get comment details
- `POST /api/comment` - Create comment
- `PUT /api/comment/{id}` - Update comment
- `DELETE /api/comment/{id}` - Delete comment

## Testing Strategy

### Unit Tests
- Controller-level tests using in-memory database
- Mock IUserContext for ownership simulation
- Test coverage includes:
  - Happy paths (successful operations)
  - Negative cases (404 for non-owned resources)
  - Invalid input handling
  - Edge cases (empty results, filtering)

### Test Data
Test data includes:
- 2 users (alice: ID=1, bob: ID=2)
- 2 projects (1 owned by alice, 1 owned by bob)
- 2 tasks (1 in each project)
- 1 comment (on task 1)

## Files Created/Modified

### New Files Created
- `DTOs/ProjectDtos.cs`
- `DTOs/TaskDtos.cs`
- `DTOs/CommentDtos.cs`
- `Services/IProjectService.cs`
- `Services/ProjectService.cs`
- `Services/ITaskService.cs`
- `Services/TaskService.cs`
- `Services/ICommentService.cs`
- `Services/CommentService.cs`
- `Controllers/ProjectController.cs`
- `Controllers/TaskController.cs`
- `Controllers/CommentController.cs`
- `TaskManagementAPI.Tests/Helpers/FakeUserContext.cs`
- `TaskManagementAPI.Tests/Helpers/TestDbContextFactory.cs`
- `TaskManagementAPI.Tests/Controllers/ProjectControllerTests.cs`
- `TaskManagementAPI.Tests/Controllers/TaskControllerTests.cs`
- `TaskManagementAPI.Tests/Controllers/CommentControllerTests.cs`

### Modified Files
- `Program.cs` - Added service registrations for ProjectService, TaskService, and CommentService

## Conclusion

All required functionality has been implemented:
✅ Project CRUD operations with ownership enforcement
✅ Task CRUD operations with ownership enforcement
✅ Task status updates
✅ Task assignment functionality
✅ Comment CRUD operations with ownership enforcement
✅ Thin controllers with business logic in services
✅ Unit tests for all controllers
✅ Proper error handling and HTTP status codes
✅ JWT authentication on all endpoints


