# Task Management API - Part 2: Detailed Implementation Plan

## Table of Contents

1. [Project Overview](#project-overview)
2. [Technology Stack & Architecture](#technology-stack--architecture)
3. [Project Structure](#project-structure)
4. [Stage 1: Project Preparation & Understanding](#stage-1-project-preparation--understanding)
5. [Stage 2: Blazor Frontend - Authentication Module](#stage-2-blazor-frontend---authentication-module)
6. [Stage 3: Blazor Frontend - Project Management Module](#stage-3-blazor-frontend---project-management-module)
7. [Stage 4: Blazor Frontend - Task Management Module](#stage-4-blazor-frontend---task-management-module)
8. [Stage 5: Blazor Frontend - UI/UX Polish](#stage-5-blazor-frontend---uiux-polish)
9. [Stage 6: Comprehensive API Testing](#stage-6-comprehensive-api-testing)
10. [Stage 7: Documentation](#stage-7-documentation)
11. [Stage 8: Delivery Preparation](#stage-8-delivery-preparation)

---

## Project Overview

### Purpose
You will develop a Blazor Server-side web application that serves as a frontend for the Task Management API (Part 1). The application will allow authenticated users to manage their projects and tasks through an intuitive user interface. Additionally, you will expand the existing test coverage to achieve ~100% coverage on all custom controllers.

### Scope
- **Blazor Frontend**: Complete CRUD operations for projects and tasks
- **Authentication**: Login/logout with JWT token management
- **Testing**: Comprehensive test coverage for API controllers
- **Documentation**: Code explanations, test setup, and personal reflection

### Key Dates & Milestones
- **Estimated Duration**: 4-6 weeks (depending on pace)
- **Milestone 1**: Blazor project setup + authentication (Week 1-2)
- **Milestone 2**: Project & Task CRUD implementation (Week 2-3)
- **Milestone 3**: UI/UX polish + testing (Week 3-4)
- **Milestone 4**: Documentation + delivery (Week 4-5)

---

## Technology Stack & Architecture

### Confirmed Technology Choices

```
Frontend:
  ├── Blazor Server (ASP.NET Core 8)
  ├── HttpClient (C#-based HTTP communication)
  └── localStorage (JWT token persistence)

Backend (Already Complete):
  ├── ASP.NET Core 8 Web API
  ├── Entity Framework Core (SQLite)
  ├── JWT Authentication (Bearer tokens)
  └── Service Layer Pattern with Ownership Enforcement

Testing:
  ├── xUnit (existing)
  ├── In-memory SQLite database
  └── FakeUserContext for mocking

Communication:
  ├── HTTP/HTTPS requests
  ├── JWT Bearer token in Authorization header
  └── JSON request/response bodies
```

### Architecture Diagram

```mermaid
graph TB
    User["👤 User/Browser"]
    Blazor["🔷 Blazor Server App<br/>(C# Components)"]
    LocalStorage["💾 localStorage<br/>(JWT Token)"]
    API["🔌 Task Management API<br/>(ASP.NET Core)"]
    DB["🗄️ SQLite Database"]
    
    User -->|"Interacts with UI"| Blazor
    Blazor -->|"Store/Retrieve"| LocalStorage
    LocalStorage -->|"JWT Token"| Blazor
    Blazor -->|"HTTP + Bearer Token"| API
    API -->|"Query/Update"| DB
    API -->|"JSON Response"| Blazor
    
    style Blazor fill:#6c3df5
    style User fill:#ffb347
    style LocalStorage fill:#87ceeb
    style API fill:#90ee90
    style DB fill:#dda0dd
```

### Data Flow Example: Login Process

```mermaid
sequenceDiagram
    participant User as User
    participant Blazor as Blazor App
    participant API as API Server
    participant DB as Database
    participant LocalStorage as localStorage

    User->>Blazor: Enter username & password
    Blazor->>API: POST /api/auth/login
    API->>DB: Verify credentials
    DB-->>API: User found & verified
    API->>API: Generate JWT token
    API-->>Blazor: Return token + userId + username
    Blazor->>LocalStorage: Save JWT token
    Blazor->>User: Redirect to dashboard
```

---

## Project Structure

### Solution Layout After Part 2 Completion

```
TaskManagementAPI/
│
├── TaskManagementAPI/                    (Existing API Project)
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ProjectController.cs
│   │   ├── TaskController.cs
│   │   └── CommentController.cs
│   ├── Models/
│   ├── Services/
│   ├── DTOs/
│   ├── Data/
│   ├── Migrations/
│   └── Program.cs
│
├── TaskManagementAPI.Tests/              (Existing Test Project)
│   ├── Controllers/
│   │   ├── ProjectControllerTests.cs     (Will expand)
│   │   ├── TaskControllerTests.cs        (Will expand)
│   │   └── CommentControllerTests.cs     (Will expand)
│   ├── Helpers/
│   ├── AuthControllerTests.cs            (NEW - for ~100% coverage)
│   └── TaskManagementAPI.Tests.csproj
│
├── TaskManagementAPI.Blazor/             (NEW - Blazor Project)
│   ├── Components/
│   │   ├── Auth/
│   │   │   ├── LoginComponent.razor
│   │   │   └── LogoutButton.razor
│   │   ├── Projects/
│   │   │   ├── ProjectListComponent.razor
│   │   │   ├── ProjectDetailComponent.razor
│   │   │   ├── CreateProjectComponent.razor
│   │   │   └── EditProjectComponent.razor
│   │   ├── Tasks/
│   │   │   ├── TaskListComponent.razor
│   │   │   ├── TaskDetailComponent.razor
│   │   │   ├── CreateTaskComponent.razor
│   │   │   ├── EditTaskComponent.razor
│   │   │   └── TaskStatusComponent.razor
│   │   ├── Layout/
│   │   │   ├── MainLayout.razor
│   │   │   ├── Navbar.razor
│   │   │   └── Sidebar.razor
│   │   └── Shared/
│   │       ├── LoadingSpinner.razor
│   │       ├── ErrorAlert.razor
│   │       └── SuccessToast.razor
│   ├── Services/
│   │   ├── IApiClient.cs
│   │   ├── ApiClient.cs
│   │   ├── AuthenticationService.cs
│   │   ├── ProjectApiService.cs
│   │   ├── TaskApiService.cs
│   │   └── LocalStorageService.cs
│   ├── Pages/
│   │   ├── Dashboard.razor
│   │   ├── ProjectPage.razor
│   │   └── TaskPage.razor
│   ├── wwwroot/
│   │   ├── css/
│   │   │   ├── app.css
│   │   │   └── bootstrap-custom.css
│   │   └── js/
│   ├── App.razor
│   ├── Program.cs
│   └── TaskManagementAPI.Blazor.csproj
│
├── IMPLEMENTATION_PLAN_PART2.md          (This file)
├── TaskManagementAPI.sln
└── README.md
```

### File Creation Checklist

- [ ] **Blazor Project**: `TaskManagementAPI.Blazor` (new)
- [ ] **Components**: 10+ Razor components (Auth, Projects, Tasks, Layout)
- [ ] **Services**: 5-6 C# service classes for API communication
- [ ] **Pages**: 3 main pages (Dashboard, Projects, Tasks)
- [ ] **Tests**: Additional test classes for AuthController
- [ ] **Configuration**: appsettings.json, Program.cs setup
- [ ] **Styling**: Basic CSS for user-friendly interface

---

## Stage 1: Project Preparation & Understanding

### Overview
In this stage, you'll set up the Blazor project infrastructure, understand the API endpoints, and prepare your development environment.

### Duration: 2-3 days

### Objectives
- ✅ Create a new Blazor Server project within the solution
- ✅ Review and document all API endpoints
- ✅ Understand JWT token flow and storage requirements
- ✅ Set up project dependencies and configuration
- ✅ Create the foundation for API communication

### Tasks

#### Task 1.1: Create Blazor Server Project

**What to do:**
Create a new Blazor Server project targeting .NET 8 and add it to your solution.

**Commands to run:**

```bash
# Navigate to your project root
cd D:\StudioProjects\TaskManagementAPI

# Create new Blazor Server project
dotnet new blazor --interactivity Server --output TaskManagementAPI.Blazor --auth None

# Add project to solution
dotnet sln add .\TaskManagementAPI.Blazor\TaskManagementAPI.Blazor.csproj
```

**Expected Output:**
- New folder: `TaskManagementAPI.Blazor/`
- Files: `App.razor`, `Program.cs`, `appsettings.json`, `.csproj`, etc.
- Project should appear in Visual Studio Solution Explorer

**Verification:**
```bash
# Verify the project builds
dotnet build
```

✅ **Completion Criteria**: No build errors, project appears in solution explorer

---

#### Task 1.2: Add Required NuGet Packages to Blazor Project

**What to do:**
Add necessary packages for HTTP communication, local storage, and JWT token handling.

**Packages to add:**

| Package | Version | Purpose |
|---------|---------|---------|
| `Blazored.LocalStorage` | Latest | Browser localStorage access in Blazor |
| `System.IdentityModel.Tokens.Jwt` | 7.* | JWT token parsing |
| `Microsoft.AspNetCore.Components.Authorization` | 8.* | Built-in (should exist) |

**Commands to run:**

```bash
cd TaskManagementAPI.Blazor

# Add local storage package
dotnet add package Blazored.LocalStorage

# Add JWT package (if not already present)
dotnet add package System.IdentityModel.Tokens.Jwt

# Restore all dependencies
dotnet restore
```

**Expected Output:**
- Packages added to `TaskManagementAPI.Blazor.csproj`
- No dependency conflicts

✅ **Completion Criteria**: Packages installed successfully, no warnings

---

#### Task 1.3: Create API Endpoint Reference Document

**What to do:**
Create a local reference document listing all API endpoints with their request/response formats.

**File to create:** `TaskManagementAPI.Blazor/API_ENDPOINTS_REFERENCE.md`

**Details:** See the created file `TaskManagementAPI.Blazor/API_ENDPOINTS_REFERENCE.md` in your project

**Reference this document:**
- Base URL configuration
- All authentication endpoints (register, login)
- All project endpoints (CRUD, search)
- All task endpoints (CRUD, status, assign)
- Error response formats
- HTTP status codes

✅ **Completion Criteria**: Reference document created and accessible in project

---

#### Task 1.4: Create Local Development Configuration

**What to do:**
Update Blazor project configuration to communicate with the API.

**Files to create/modify:**
- `TaskManagementAPI.Blazor/appsettings.json`
- `TaskManagementAPI.Blazor/appsettings.Development.json`

**Configuration includes:**
- API base URL: `https://localhost:5001`
- Request timeout: 30 seconds
- Logging levels (Information for prod, Debug for dev)

**See the created files** in your project folder.

✅ **Completion Criteria**: Configuration files created/updated with proper settings

---

#### Task 1.5: Create Base API Client Service

**What to do:**
Create a foundational service class for making HTTP calls to the API with JWT token handling.

**Files to create:**
- `TaskManagementAPI.Blazor/Services/IApiClient.cs` (Interface)
- `TaskManagementAPI.Blazor/Services/ApiClient.cs` (Implementation)

**Key responsibilities:**
- Set/Clear JWT tokens in Authorization headers
- Handle GET, POST, PUT, PATCH, DELETE requests
- Deserialize JSON responses to typed objects
- Log requests and errors
- Handle HTTP error responses with proper exceptions

**Key methods:**
- `SetTokenAsync(string token)` - Configure Bearer token
- `ClearTokenAsync()` - Remove token
- `GetAsync<T>(string endpoint)` - GET request
- `PostAsync<T>(string endpoint, object body)` - POST request
- `PutAsync<T>(string endpoint, object body)` - PUT request
- `PatchAsync<T>(string endpoint, object body)` - PATCH request
- `DeleteAsync(string endpoint)` - DELETE request

**See the created files** in your `Services/` folder.

✅ **Completion Criteria**: IApiClient interface and ApiClient implementation created and compile without errors

---

#### Task 1.6: Summary & Next Steps

**What you've accomplished:**
- ✅ Created new Blazor Server project
- ✅ Added necessary NuGet packages
- ✅ Created API endpoint reference document
- ✅ Configured appsettings for API communication
- ✅ Created base ApiClient service for HTTP communication

**Checkpoint:**
- Verify build succeeds: `dotnet build`
- Verify no compilation errors
- All files are created and in place

**Next in Stage 2:**
- Create Authentication Service
- Create Login/Logout components
- Implement JWT token storage in localStorage
- Create route protection

---

## Stage 2: Blazor Frontend - Authentication Module

### Overview
Implement complete authentication flow: login page, JWT token management, secure storage, and session handling.

### Duration: 3-4 days

### Objectives
- ✅ Create login and registration pages
- ✅ Implement authentication service with API calls
- ✅ Handle JWT token storage in localStorage
- ✅ Create logout functionality
- ✅ Protect routes with authentication check

### Dependencies
- IApiClient (from Stage 1)
- Blazored.LocalStorage package

### Key Concepts

#### JWT Token Flow in Blazor
```mermaid
flowchart LR
    A["User Enters<br/>Credentials"] -->|"Submit Form"| B["LoginComponent"]
    B -->|"Call LoginAsync()"| C["AuthenticationService"]
    C -->|"POST /api/auth/login"| D["API Server"]
    D -->|"Validate & Create JWT"| E["Return Token"]
    E -->|"AuthResultDto"| C
    C -->|"Save Token"| F["localStorage"]
    F -->|"Set Authorization<br/>Header"| C
    C -->|"Success Signal"| B
    B -->|"Redirect"| G["Dashboard"]
```

#### localStorage Usage
```csharp
// Save token after successful login
await localStorageService.SetItemAsync("authToken", token);

// Retrieve token on app startup
var token = await localStorageService.GetItemAsync<string>("authToken");

// Remove token on logout
await localStorageService.RemoveItemAsync("authToken");
```

### Tasks

#### Task 2.1: Create Authentication Service

**File to create:** `TaskManagementAPI.Blazor/Services/AuthenticationService.cs`

**Key responsibilities:**
- Register new users (POST to `/api/auth/register`)
- Login existing users (POST to `/api/auth/login`)
- Logout users (clear token from storage and API client)
- Initialize authentication on app startup (restore token from localStorage)
- Check if user is authenticated
- Get current user's token

**Key methods:**
- `RegisterAsync(username, email, password)` - User registration
- `LoginAsync(username, password)` - User login
- `LogoutAsync()` - User logout
- `InitializeAsync()` - Restore token on app startup
- `IsAuthenticatedAsync()` - Check if authenticated
- `GetTokenAsync()` - Get current token

**Event:**
- `OnAuthStateChanged` - Event fired when authentication state changes (notify other components)

**Request/Response models:**
- `RegisterRequest` - Registration request (username, email, password)
- `LoginRequest` - Login request (username, password)
- `AuthResultDto` - API response (token, userId, username)
- `AuthResult` - Result object for operations (success, message, user details)

**See the created file** in your `Services/` folder.

✅ **Completion Criteria**: AuthenticationService created and compiles without errors

---

#### Task 2.2: Create DTOs and Models Folder

**What to do:**
Create a Models folder structure in the Blazor project for data classes.

**Create folder:** `TaskManagementAPI.Blazor/Models/`

**Files to create:**
- `TaskManagementAPI.Blazor/Models/AuthModels.cs`

**Models included:**
- `AuthResultDto` - Authentication response from API (token, userId, username)
- `LoginDto` - Login request data (username, password)
- `RegisterDto` - Registration request data (username, email, password)

**See the created file** in your `Models/` folder.

✅ **Completion Criteria**: Models folder and files created

---

#### Task 2.3: Register Services in Program.cs

**What to do:**
Register all new services in Blazor's dependency injection container.

**File to modify:** `TaskManagementAPI.Blazor/Program.cs`

**Services to register:**
- `AddAuthorizationCore()` - Enable authorization
- `AddCascadingAuthenticationState()` - Share auth state across components
- `AddBlazoredLocalStorage()` - Enable localStorage access
- `HttpClient` with base address configured
- `IApiClient` → `ApiClient` (scoped)
- `AuthenticationService` (scoped)

**Configuration details:**
- Base URL: https://localhost:5001
- Scoped lifetime for services (new instance per request)

**See the file** in your project root for implementation details.

✅ **Completion Criteria**: Program.cs updated with service registrations

---

#### Task 2.4: Create Login Component

**What to do:**
Create a Razor component for user login.

**File to create:** `TaskManagementAPI.Blazor/Components/Auth/LoginComponent.razor`

**Component features:**
- Login form with username and password fields
- Form validation using `EditForm` and `DataAnnotationsValidator`
- Error message display
- Loading spinner during authentication
- Submit button that calls `AuthenticationService.LoginAsync()`
- Navigation to dashboard on successful login
- Link to registration page

**Layout:**
- Centered card layout with purple gradient background
- Professional styling with Bootstrap classes
- Responsive design

**See the created file** in your `Components/Auth/` folder.

✅ **Completion Criteria**: LoginComponent created and syntax is valid

---

#### Task 2.5: Create Registration Component

**File to create:** `TaskManagementAPI.Blazor/Components/Auth/RegisterComponent.razor`

**Component features:**
- Registration form with username, email, and password fields
- Form validation
- Error message display
- Loading spinner during registration
- Submit button that calls `AuthenticationService.RegisterAsync()`
- Navigation to dashboard on successful registration
- Link to login page

**Layout:**
- Centered card layout with purple gradient background
- Professional styling with Bootstrap classes
- Responsive design
- Similar styling to LoginComponent for consistency

**See the created file** in your `Components/Auth/` folder.

✅ **Completion Criteria**: RegisterComponent created and syntax is valid

---

## Stage 3: Blazor Frontend - Project Management Module

### Overview
Implement complete CRUD operations for projects. Users will be able to view, create, update, and delete their projects through Blazor components with a clean, intuitive interface.

### Duration: 4-5 days

### Objectives
- ✅ Create service for project API communication
- ✅ Create components for listing, creating, updating, and deleting projects
- ✅ Implement search functionality
- ✅ Handle loading states and error messages
- ✅ Navigate between project views

### Dependencies
- AuthenticationService (from Stage 2)
- IApiClient (from Stage 1)
- Bootstrap CSS for styling

### Key Concepts

#### Project Management Flow
```mermaid
flowchart TD
    A["Dashboard"] -->|"View Projects"| B["ProjectListComponent"]
    B -->|"Click Create"| C["CreateProjectComponent"]
    C -->|"POST /api/project"| D["API"]
    D -->|"Success"| B
    B -->|"Click Edit"| E["EditProjectComponent"]
    E -->|"PUT /api/project/{id}"| D
    B -->|"Click Delete"| F["Delete Confirmation"]
    F -->|"DELETE /api/project/{id}"| D
    B -->|"Search"| B
```

### Tasks

#### Task 3.1: Create Project API Service

**File to create:** `TaskManagementAPI.Blazor/Services/ProjectApiService.cs`

**Key responsibilities:**
- Communicate with Project endpoints (`GET`, `POST`, `PUT`, `DELETE`)
- Handle project-related API calls
- Map API responses to DTOs
- Provide methods: GetProjectsAsync, GetProjectByIdAsync, CreateProjectAsync, UpdateProjectAsync, DeleteProjectAsync, SearchProjectsAsync

**Dependencies injected:**
- `IApiClient` - HTTP communication
- `ILogger<ProjectApiService>` - Logging

**See the created file** in your `Services/` folder.

✅ **Completion Criteria**: ProjectApiService created with all project-related methods

---

#### Task 3.2: Create Project DTOs

**File to create:** `TaskManagementAPI.Blazor/Models/ProjectModels.cs`

**DTOs to include:**
- `ProjectDto` - Response from API (Id, Name, Description, CreatedDate, UserId, OwnerUsername, TaskCount)
- `CreateProjectDto` - Request for creating project (Name, Description)
- `UpdateProjectDto` - Request for updating project (Name, Description)

**See the created file** in your `Models/` folder.

✅ **Completion Criteria**: Project DTOs created

---

#### Task 3.3: Create Project List Component

**File to create:** `TaskManagementAPI.Blazor/Components/Projects/ProjectListComponent.razor`

**Component features:**
- Display list of user's projects in table or card format
- Search bar to filter projects by name
- Create new project button
- Edit button for each project
- Delete button for each project (with confirmation)
- Loading spinner while fetching
- Error message display
- Empty state message when no projects

**Layout:**
- Header with search bar and create button
- Project list (table or cards)
- Pagination (optional for MVP)

**See the created file** in your `Components/Projects/` folder.

✅ **Completion Criteria**: ProjectListComponent displays projects and handles user interactions
✅ **STATUS: COMPLETED** - ProjectListComponent.razor created with full functionality

---

#### Task 3.4: Create Project Create Component

**File to create:** `TaskManagementAPI.Blazor/Components/Projects/CreateProjectComponent.razor`

**Component features:**
- Form with Name and Description fields
- Form validation
- Submit button
- Cancel button (navigate back)
- Loading indicator during submission
- Error message display
- Success redirect to project list

**Form fields:**
- Name (required, max 100 chars)
- Description (optional, max 500 chars)

**See the created file** in your `Components/Projects/` folder.

✅ **Completion Criteria**: CreateProjectComponent functional with validation
✅ **STATUS: COMPLETED** - CreateProjectComponent.razor created with EditForm and validation

---
#### Task 3.5: Create Project Edit Component

**File to create:** `TaskManagementAPI.Blazor/Components/Projects/EditProjectComponent.razor`

**Component features:**
- Load existing project data on init
- Form with Name and Description fields
- Form validation
- Update button
- Cancel button
- Loading indicator
- Error message display
- Success redirect to project list

**Navigation:**
- Accept project ID via route parameter
- Load project data from API
- Pre-fill form with existing data

**See the created file** in your `Components/Projects/` folder.

✅ **Completion Criteria**: EditProjectComponent loads and updates projects
✅ **STATUS: COMPLETED** - EditProjectComponent.razor created with route parameters and pre-filled form
✅ **Completion Criteria**: EditProjectComponent loads and updates projects

---
#### Task 3.6: Create Project Detail Component

**File to create:** `TaskManagementAPI.Blazor/Components/Projects/ProjectDetailComponent.razor`

**Component features:**
- Display project details (name, description, owner, creation date)
- Show count of tasks in project
- Link to view/manage project tasks
- Edit button
- Delete button
- Back to list button

**Navigation:**
- Accept project ID via route parameter
- Load and display project information

**See the created file** in your `Components/Projects/` folder.

✅ **Completion Criteria**: ProjectDetailComponent displays project information
✅ **STATUS: COMPLETED** - ProjectDetailComponent.razor created with all features
✅ **Completion Criteria**: ProjectDetailComponent displays project information

---

#### Task 3.7: Update App.razor with Routes

**Files to create/modify:**
- `TaskManagementAPI.Blazor/App.razor` - Main app shell (HTML structure)
- `TaskManagementAPI.Blazor/Routes.razor` - Router configuration
- `TaskManagementAPI.Blazor/Components/Layouts/MainLayout.razor` - Main page layout
- `TaskManagementAPI.Blazor/Components/Layouts/AuthLayout.razor` - Auth pages layout
- `TaskManagementAPI.Blazor/Components/Pages/HomePage.razor` - Home/redirect page
- `TaskManagementAPI.Blazor/Components/Pages/LoginPage.razor` - Login page
- `TaskManagementAPI.Blazor/Components/Pages/RegisterPage.razor` - Register page
- `TaskManagementAPI.Blazor/Components/_Imports.razor` - Global using directives

**Routes created:**
- `/` - Home page (redirects to /projects if authenticated, /login otherwise)
- `/login` - Login page
- `/register` - Registration page
- `/projects` - Project list page
- `/projects/create` - Create project page
- `/projects/{id}` - Project detail page
- `/projects/{id}/edit` - Edit project page

**Layout structure:**
- AuthLayout for unauthenticated pages (login, register)
- MainLayout for authenticated pages (projects, tasks)
- Navbar with navigation links and logout
- Footer with copyright

✅ **Completion Criteria**: All routes configured and app structure complete
✅ **STATUS: COMPLETED** - App.razor, routing, layouts, and pages created

---

#### Task 3.8: Summary & Next Steps

**What you've accomplished:**
- ✅ Created ProjectApiService for API communication (Task 3.1)
- ✅ Created Project DTOs for type safety (Task 3.2)
- ✅ Implemented ProjectListComponent with search (Task 3.3)
- ✅ Implemented CreateProjectComponent with validation (Task 3.4)
- ✅ Implemented EditProjectComponent with data loading (Task 3.5)
- ✅ Implemented ProjectDetailComponent for viewing (Task 3.6)
- ✅ Configured App.razor, routing, and layouts (Task 3.7)

**Stage 3 Completion Status:** ✅ ALL TASKS COMPLETE

---

---

## Stage 4: Task Management Module

**Objective**: Implement complete task management functionality (CRUD operations, status tracking, assignment)

**Key Features**:
- Display all user tasks with filtering (by project, status, search)
- Create new tasks with validation
- Edit existing tasks
- Update task status (To Do → In Progress → Done)
- Delete tasks
- Task detail view with all information

**Architecture**:
```
Tasks
├── TaskApiService (API communication)
│   ├── GetTasks() - Fetch all tasks
│   ├── GetTasksByProject(projectId) - Filter by project
│   ├── GetTasksByStatus(status) - Filter by status
│   ├── GetTaskById(id) - Single task detail
│   ├── CreateTask(taskData) - Create new task
│   ├── UpdateTask(id, taskData) - Update task
│   ├── UpdateTaskStatus(id, status) - Change status
│   └── DeleteTask(id) - Remove task
├── TaskModels.cs (Data Transfer Objects)
│   ├── TaskDto - Response object
│   ├── CreateTaskDto - Create request
│   ├── UpdateTaskDto - Update request
│   ├── TaskStatus - Enum (ToDo, InProgress, Done)
│   └── Helper DTOs for status/assignment
└── Components
    ├── TaskListComponent - List with filtering
    ├── CreateTaskComponent - Create form
    ├── EditTaskComponent - Update form
    └── TaskDetailComponent - Task view
```

**Routes**:
- `/tasks` - Task list page
- `/tasks/create` - Create task page
- `/tasks/{id}` - Task detail page
- `/tasks/{id}/edit` - Edit task page

---

#### Task 4.1: Create Task Models and DTOs

**File to create:** `TaskManagementAPI.Blazor/Models/TaskModels.cs`

**Contains:**
- `TaskStatus` enum (ToDo=0, InProgress=1, Done=2)
- `TaskDto` - API response (includes Id, Title, Description, Status, DueDate, ProjectId, AssignedUserId, etc.)
- `CreateTaskDto` - Create request (Title, Description, Status, DueDate, ProjectId, AssignedUserId)
- `UpdateTaskDto` - Update request (same as CreateTaskDto)
- Helper DTOs: UpdateTaskStatusDto, AssignTaskDto

**See the created file** in your `Models/` folder.

✅ **Completion Criteria**: All task DTOs defined with proper properties
✅ **STATUS: COMPLETED** - TaskModels.cs created with all classes

---

#### Task 4.2: Create Task API Service

**File to create:** `TaskManagementAPI.Blazor/Services/TaskApiService.cs`

**Methods to implement:**
- `GetTasksAsync()` - Fetch all tasks
- `GetTasksByProjectAsync(projectId)` - Filter by project
- `GetTasksByStatusAsync(status)` - Filter by status
- `GetTaskByIdAsync(id)` - Get single task
- `CreateTaskAsync(taskData)` - Create new task
- `UpdateTaskAsync(id, taskData)` - Update task
- `UpdateTaskStatusAsync(id, status)` - Change status
- `AssignTaskAsync(id, userId)` - Assign to user
- `DeleteTaskAsync(id)` - Delete task

**Key responsibilities:**
- Communicate with Task API endpoints
- Handle errors and logging
- Return strongly-typed objects

**See the created file** in your `Services/` folder.

✅ **Completion Criteria**: TaskApiService fully implemented with all CRUD methods
✅ **STATUS: COMPLETED** - TaskApiService.cs created

---

#### Task 4.3: Create Task List Component

**File to create:** `TaskManagementAPI.Blazor/Components/Tasks/TaskListComponent.razor`

**Component features:**
- Display all user tasks in card grid layout
- Search bar to filter by title/description
- Project filter dropdown
- Status filter dropdown
- "New Task" button
- View, Edit, Delete buttons for each task
- Status badge with color coding (red=To Do, orange=In Progress, green=Done)
- Loading spinner and error handling
- Delete confirmation modal

**Filter behavior:**
- Real-time filtering as user types/changes filters
- Multiple filters can be applied simultaneously
- Search is case-insensitive

**Route**: `/tasks`

**See the created file** in your `Components/Tasks/` folder.

✅ **Completion Criteria**: TaskListComponent displays tasks with all filters working
✅ **STATUS: COMPLETED** - TaskListComponent.razor created with search and filters

---

#### Task 4.4: Create Task Create Component

**File to create:** `TaskManagementAPI.Blazor/Components/Tasks/CreateTaskComponent.razor`

**Component features:**
- Form with fields: Project (dropdown), Title, Description, Status, Due Date
- Form validation (Title required)
- Project dropdown auto-populated from ProjectApiService
- Status defaults to "To Do"
- Submit and Cancel buttons
- Loading indicator during submission
- Error message display
- Success redirect to `/tasks`

**Form fields:**
- Project (required, dropdown)
- Title (required, text)
- Description (optional, textarea)
- Status (dropdown, default: To Do)
- Due Date (optional, date picker)

**Route**: `/tasks/create`

**See the created file** in your `Components/Tasks/` folder.

✅ **Completion Criteria**: CreateTaskComponent functional with validation and project dropdown
✅ **STATUS: COMPLETED** - CreateTaskComponent.razor created

---

#### Task 4.5: Create Task Edit Component

**File to create:** `TaskManagementAPI.Blazor/Components/Tasks/EditTaskComponent.razor`

**Component features:**
- Load existing task data on init
- Form with same fields as CreateTaskComponent
- Pre-fill form with existing data
- Project dropdown
- Update button
- Cancel button
- Loading indicator
- Error message display
- Success redirect to `/tasks`

**Navigation:**
- Accept task ID via route parameter `{id:int}`
- Load task data from API
- Pre-fill all form fields

**Route**: `/tasks/{id}/edit`

**See the created file** in your `Components/Tasks/` folder.

✅ **Completion Criteria**: EditTaskComponent loads and updates tasks
✅ **STATUS: COMPLETED** - EditTaskComponent.razor created

---

#### Task 4.6: Create Task Detail Component

**File to create:** `TaskManagementAPI.Blazor/Components/Tasks/TaskDetailComponent.razor`

**Component features:**
- Display complete task information
- Show: Title, Description, Status (with color badge), Project link, Created date, Created by, Due date, Assigned user
- Status dropdown for quick status change (without page reload)
- Edit button (routes to edit page)
- Delete button (with confirmation modal)
- Back to list button
- Loading spinner and error handling
- Real-time status update feedback

**Navigation:**
- Accept task ID via route parameter `{id:int}`
- Load task data from API

**Route**: `/tasks/{id}`

**See the created file** in your `Components/Tasks/` folder.

✅ **Completion Criteria**: TaskDetailComponent displays all task info with status change capability
✅ **STATUS: COMPLETED** - TaskDetailComponent.razor created

---

#### Task 4.7: Update Navigation and Routes

**Files to modify:**
- `TaskManagementAPI.Blazor/Components/Layouts/MainLayout.razor` - Add "Tasks" link to navbar
- Routes are automatically discovered by Router component (no manual routing needed)

**Navigation items to add:**
- "Tasks" link pointing to `/tasks` in the main navbar (between Projects and Profile)

**Routes that auto-register:**
- `/tasks` - TaskListComponent
- `/tasks/create` - CreateTaskComponent
- `/tasks/{id}` - TaskDetailComponent
- `/tasks/{id}/edit` - EditTaskComponent

**See the modified file** in your `Components/Layouts/` folder.

✅ **Completion Criteria**: Navigation includes Tasks link and all routes accessible
✅ **STATUS: COMPLETED** - MainLayout.razor updated with Tasks navigation

---

#### Task 4.8: Summary & Next Steps

**What you've accomplished in Stage 4:**
- ✅ Created TaskModels.cs with all DTOs (Task 4.1)
- ✅ Implemented TaskApiService with all CRUD methods (Task 4.2)
- ✅ Implemented TaskListComponent with search and filtering (Task 4.3)
- ✅ Implemented CreateTaskComponent with validation (Task 4.4)
- ✅ Implemented EditTaskComponent with data loading (Task 4.5)
- ✅ Implemented TaskDetailComponent with status management (Task 4.6)
- ✅ Updated navigation with Tasks link (Task 4.7)

**Stage 4 Completion Status:** ✅ ALL TASKS COMPLETE

**Stage 4 Final Checkpoint - All Items Verified:**
- ✅ App structure created with all component scaffolding (19 total components)
- ✅ Navigation and routing configured (11 routes implemented)
- ✅ Layout components for authenticated/unauthenticated pages (MainLayout, AuthLayout)
- ✅ All project CRUD components implemented (4 components: List, Create, Edit, Detail)
- ✅ All task CRUD components implemented (4 components: List, Create, Edit, Detail)
- ✅ Task management components with filtering and search (Project & Status filters)
- ✅ TaskApiService with all operations (9 methods: Get, GetByProject, GetByStatus, GetById, Create, Update, UpdateStatus, Assign, Delete)
- ✅ Task CRUD operations fully functional (Create, Read, Update, Delete)
- ✅ Task status updates with real-time feedback (To Do → In Progress → Done)
- ✅ Task assignment functionality
- ✅ Build succeeds with 0 errors
- ✅ Application runs successfully on localhost:5114

**Manual Testing Checklist:**
- [ ] Verify all components compile: `dotnet build`
- [ ] Test creating a task
- [ ] Test editing a task
- [ ] Test changing task status
- [ ] Test filtering tasks by project and status
- [ ] Test deleting a task
- [ ] Verify navigation between projects and tasks
- [ ] Test responsive design on mobile devices
- [ ] Verify error handling and user feedback

**Next in Stage 5: Blazor Frontend - UI/UX Polish.**
- Add UI/UX polish and shared components: Navigation menu/sidebar if needed.
- Implement consistent loading states across application (not sure what is meant here).
- Add error handling and user feedback components (alerts, toasts).
- Add data validation feedback on forms.
- Create shared component library (loading indicators for async operations, error alerts, success toasts).
- Optionally: Improve responsive design for mobile devices.
  
**Next in Stage 6: Comprehensive API Testing**
 - Expand existing test coverage for ProjectController
 - Expand existing test coverage for TaskController
 - Expand existing test coverage for CommentController
 - Add edge case tests (empty results, invalid inputs, authorization failures)
 - Achieve ~100% code coverage on custom controllers
 - Document test scenarios and setup

**Next in Stage 7: Documentation**
- Write brief task description
-  Document Blazor frontend architecture and structure
-  Document any API improvements/changes made
-  Create test documentation (scenarios, setup, coverage report)
-  Write personal reflection on the process

**Next in Stage 8: Delivery Preparation**
- Record video demo of working Blazor application
- Verify all source code is clean and committed
- Generate PDF documentation
- Create zip file with all required materials

---