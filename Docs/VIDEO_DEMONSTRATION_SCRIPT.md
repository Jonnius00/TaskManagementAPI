# Video Demonstration Script
## Task Management System - Full-Stack Application

**Target Duration**: 10-12 minutes  
**Date**: December 2025  
**Purpose**: Demonstrate Part 2 implementation (Blazor frontend + comprehensive testing)

---

## 🎬 INTRODUCTION (1 minute)

### Opening [0:00-0:30]

**[Show desktop with project folder open]**

> "Hello! In this video, I'll demonstrate the Task Management System I built for Part 2 of the assignment. This is a full-stack .NET 8 application consisting of a RESTful API backend and a Blazor Server frontend with comprehensive unit testing."

**[Show README.md open in VS Code]**

> "The project has three main components: First, the backend API with JWT authentication that we built in Part 1. Second, the new Blazor Server frontend that provides an interactive web interface. And third, a comprehensive test suite with 74 unit tests achieving approximately 95% code coverage across all controllers."

### Project Overview [0:30-1:00]

**[Show project structure in Explorer]**

> "Let me show you the project structure. We have the main API project here with controllers, services, and models. The Blazor frontend is in TaskManagementAPI.Blazor with components organized by feature. And the test project contains all our unit tests."

**[Scroll through folder structure slowly]**

> "The application allows users to register, login, create projects, manage tasks with different statuses, and everything is secured with JWT authentication. Users can only see and manage their own data."

---

## 💻 RUNNING THE APPLICATION (30 seconds)

### Starting the Servers [1:00-1:30]

**[Open two terminals side by side]**

> "To run the application, I need to start both the API backend and the Blazor frontend. Let me start the API first."

**[Terminal 1 - Navigate and run API]**
```powershell
cd d:\StudioProjects\TaskManagementAPI
dotnet run
```

**[Wait for "Now listening on: http://localhost:5114"]**

> "The API is now running on port 5114. Now let me start the Blazor frontend."

**[Terminal 2 - Navigate and run Blazor]**
```powershell
cd TaskManagementAPI.Blazor
dotnet run
```

**[Wait for "Now listening on: https://localhost:7148"]**

> "Perfect! The Blazor app is running on port 7148. Let me open the application in the browser."

---

## 🔐 AUTHENTICATION MODULE (2 minutes)

### Registration [1:30-2:15]

**[Open browser to https://localhost:7148]**

> "The application starts at the home page. Let me first demonstrate user registration."

**[Click "Register" in navbar]**

> "Here's the registration form. I'll create a new user to demonstrate the authentication flow."

**[Fill in registration form]**
- Username: `demo_user`
- Email: `demo@example.com`
- Password: `SecurePass123!`
- Full Name: `Demo User`

**[Click Register button]**

> "When I submit the registration, the Blazor app makes an HTTP POST request to the API's auth/register endpoint. The API validates the data, hashes the password using BCrypt, creates the user in the database, and returns a JWT token."

**[Show successful registration - redirected to home/projects page]**

> "As you can see, registration was successful and I'm automatically logged in. The JWT token is now stored in the browser's localStorage."

### Login [2:15-2:45]

**[Click Logout button]**

> "Let me log out and demonstrate the login functionality with an existing user."

**[Click "Login" in navbar]**

> "This is the login page. I'll use one of the pre-seeded test users."

**[Fill in login form]**
- Username: `alice`
- Password: `password123`

**[Click Login button]**

> "After successful authentication, the API returns a JWT token which contains the user's ID and username. This token is stored in localStorage and included in every subsequent API request in the Authorization header."

**[Show logged in state with username in navbar if visible]**

> "I'm now logged in as Alice. The authentication system ensures that I can only access my own projects and tasks."

### Token Persistence Explanation [2:45-3:00]

**[Optional: Press F12 to show DevTools → Application → Local Storage]**

> "If I open the browser's developer tools, you can see the JWT token stored in localStorage. This was one of the critical bugs I fixed during development - ensuring the token persists across page refreshes and is included in every API call."

---

## 📁 PROJECT MANAGEMENT (2.5 minutes)

### Viewing Projects [3:00-3:30]

**[Navigate to Projects page if not already there]**

> "Now let me show you the project management features. This page displays all projects owned by the current user."

**[Show existing projects in table/list]**

> "Alice has two existing projects here. For each project, we can see the name, description, creation date, and the number of tasks. The interface is built with Bootstrap 5 for responsive design."

**[Demonstrate search if available]**

> "There's a search box here that allows filtering projects by name in real-time."

**[Type in search box to filter projects]**

> "The filtering happens client-side, so it's instant."

### Creating a Project [3:30-4:15]

**[Click "Create New Project" button]**

> "Let me create a new project to demonstrate the CRUD operations."

**[Show create project form]**

> "Here's the create project form with validation. I'll fill in the details."

**[Fill in form]**
- Name: `Video Demo Project`
- Description: `This project was created during the video demonstration`

**[Click Create/Save button]**

> "When I submit, the Blazor component calls the ProjectApiService, which makes an HTTP POST request to the API. The API validates the data, checks authorization, creates the project in the database with the current user as owner, and returns the created project."

**[Show success message and new project in list]**

> "Perfect! The new project appears in the list. Notice the success notification at the top."

### Viewing Project Details [4:15-4:45]

**[Click on the new project to view details]**

> "Let me click on this project to view its details."

**[Show project detail page with task list]**

> "This is the project detail page. At the top, we see the project information - name, description, and dates. Below that is a list of all tasks belonging to this project."

**[Point to empty task list or existing tasks]**

> "Currently, this project has no tasks. We can add tasks from here or from the main tasks page."

### Editing a Project [4:45-5:15]

**[Click "Edit" button on project detail page]**

> "Let me demonstrate updating a project."

**[Show edit project form with pre-filled data]**

> "The edit form is pre-populated with the current project data. I'll make a change to the description."

**[Modify description]**
- Description: `This project was created during the video demonstration to showcase CRUD operations`

**[Click Update/Save button]**

> "After clicking update, the Blazor component sends a PUT request to the API with the modified data. The API verifies I own this project, updates it in the database, and returns the updated project."

**[Show success message and updated data]**

> "Great! The description has been updated successfully."

### Deleting a Project [5:15-5:30]

**[Navigate back to projects list]**

**[Click Delete button on a project]**

> "Finally, let me show the delete functionality. When I click delete..."

**[Show confirmation dialog]**

> "...a confirmation dialog appears to prevent accidental deletions. This is one of the shared UI components I created for better user experience."

**[Click Cancel]**

> "I'll cancel this one since we want to keep the project for task demonstrations."

---

## ✅ TASK MANAGEMENT (2.5 minutes)

### Viewing Tasks [5:30-6:00]

**[Navigate to Tasks page]**

> "Now let's look at task management. This is the task list page showing all tasks from projects I own."

**[Show task list with existing tasks]**

> "Each task displays its title, status, assigned user, due date, and which project it belongs to. Tasks can have three statuses: To Do, In Progress, or Done."

### Filtering Tasks [6:00-6:30]

**[Show filter controls]**

> "The task list has powerful filtering capabilities. I can filter by project..."

**[Select a project from dropdown]**

> "...which shows only tasks from that project."

**[Clear project filter, then select status filter]**

> "I can also filter by status - let me show only 'In Progress' tasks."

**[Select "In Progress" from status dropdown]**

> "And I can combine both filters - tasks from a specific project with a specific status."

**[Select both project and status]**

> "This was one of the comprehensive test scenarios - combined filtering. We have unit tests that verify this works correctly."

### Creating a Task [6:30-7:15]

**[Clear filters, click "Create New Task" button]**

> "Let me create a new task to demonstrate the full workflow."

**[Show create task form]**

> "The create task form includes all the necessary fields. Notice the project dropdown - I can only select projects I own."

**[Fill in task form]**
- Title: `Record demonstration video`
- Description: `Create a comprehensive video showing all features of the task management system`
- Status: To Do
- Project: `Video Demo Project`
- Due Date: [Select tomorrow's date]
- Assign to: Me (@alice)

**[Click Create/Save button]**

> "When I create a task, the API validates that the selected project exists and that I have permission to add tasks to it. It also verifies the title isn't empty and doesn't exceed 200 characters - these validation rules are thoroughly tested in our unit test suite."

**[Show success message and new task in list]**

> "Excellent! The task has been created and appears in the list with the To Do status."

### Updating Task Status [7:15-7:45]

**[Click on the newly created task to view details]**

> "Let me view this task's details and demonstrate status management."

**[Show task detail page]**

> "Here's the task detail page with all information. One of the key features is the ability to quickly update the task status."

**[Show status update buttons/dropdown]**

> "I can transition the task through different statuses. Let me mark it as In Progress."

**[Click "In Progress" button or select from dropdown]**

> "The status update sends a PATCH request to the API, which verifies I have access to this task and updates its status. Our unit tests verify that all status transitions work correctly - To Do to In Progress, In Progress to Done, and even backward transitions like Done back to In Progress."

**[Show updated status]**

> "Perfect! The task is now marked as In Progress."

### Editing a Task [7:45-8:15]

**[Click "Edit" button on task detail page]**

> "Let me demonstrate editing a task."

**[Show edit task form with pre-filled data]**

> "The edit form is pre-populated with current values. I can modify any field including moving the task to a different project."

**[Modify task]**
- Description: `Create a comprehensive 10-minute video showing all features including authentication, projects, tasks, and the testing suite`
- Status: Done

**[Click Update/Save button]**

> "After updating, the API validates the changes, checks I own both the task and the target project, and saves the changes."

**[Show updated task]**

> "Great! The task has been updated and marked as Done."

### Task Assignment [8:15-8:30]

**[Navigate back to task list or detail page]**

> "One more feature worth mentioning is task assignment. When creating or editing a task, I can assign it to myself. The dropdown shows 'Unassigned' or 'Me (@username)'. This feature required fixing a bug with JWT claims - the token uses the 'sub' claim for the user ID, which I had to handle correctly in the frontend."

---

## 🧪 TESTING DEMONSTRATION (2 minutes)

### Test Overview [8:30-9:00]

**[Switch to VS Code, open test project]**

> "Now let me show you the comprehensive testing that was implemented. The project includes 74 unit tests covering all four custom controllers."

**[Show test project structure]**

> "The tests are organized by controller: AuthController, ProjectController, TaskController, and CommentController. Each controller has extensive test coverage."

**[Open one test file, e.g., ProjectControllerTests.cs]**

> "Let me show you one of the test files. These tests use xUnit as the testing framework, an in-memory SQLite database for isolation, and a fake user context to simulate authenticated users."

### Running Tests [9:00-9:45]

**[Open terminal in test project directory]**

> "Let me run the entire test suite to show you they all pass."

**[Run command]**
```powershell
dotnet test
```

**[Wait for tests to run, show output]**

> "As you can see, all 74 tests are running. The tests cover multiple scenarios:"

**[While tests are running, explain]**

> "For authentication, we test successful registration and login, duplicate username detection, password hashing with BCrypt, and JWT token generation with proper claims."

> "For projects, we test all CRUD operations, authorization enforcement - ensuring users can't access other users' projects - validation of empty names and length constraints, cascade deletes where deleting a project also deletes its tasks, and edge cases like empty lists and non-existent resources."

> "For tasks, we test CRUD operations, filtering by project and status, combined filters, status transitions, task assignment, and authorization boundaries."

> "And for comments, we test ownership verification, ensuring users can only update or delete their own comments."

**[Show test completion - all passing]**

> "Perfect! All 74 tests passed in about 21 seconds. The test coverage is approximately 95% across all controllers, which means nearly all code paths are tested."

### Test Architecture [9:45-10:00]

**[Show TestDbContextFactory.cs or FakeUserContext.cs briefly]**

> "The test infrastructure uses two key helpers: TestDbContextFactory creates an isolated in-memory database for each test with pre-seeded users and data, and FakeUserContext simulates authenticated users without requiring real JWT tokens."

**[Optional: Show one specific test method]**

> "Here's an example test - it follows the Arrange-Act-Assert pattern: set up the test data, execute the controller action, and verify the expected outcome. This systematic approach ensures reliable, maintainable tests."

---

## 🔍 CODE QUALITY & ARCHITECTURE (1 minute)

### Project Structure [10:00-10:30]

**[Show API project structure]**

> "Let me briefly show the code architecture. The API follows clean architecture principles with clear separation of concerns."

**[Navigate through folders quickly]**

> "We have Controllers handling HTTP requests, Services containing business logic, Models defining our data structure, DTOs for data transfer, and the DbContext for database access."

**[Show Blazor project structure]**

> "The Blazor frontend is organized by feature - authentication components, project components, task components, and shared UI components. This modular structure makes the code maintainable and easy to understand."

**[Open one service file briefly, e.g., TaskApiService.cs]**

> "All API communication is centralized in service classes. These services use HttpClient with the JWT token retrieved from localStorage on every request - this pattern solved the token persistence issue I encountered during development."

---

## 🐛 CHALLENGES & SOLUTIONS (1 minute)

### Key Challenges [10:30-11:00]

**[Show Reflection.md or speak from memory]**

> "During development, I encountered several critical bugs that taught me valuable lessons."

> "The first major challenge was JWT token persistence. Initially, users would log in successfully but get 401 Unauthorized on the next API call. The problem was that Blazor Server creates new HttpClient instances per scope, so setting the Authorization header once didn't work. I solved this by retrieving the token from localStorage on every request."

> "The second challenge was JavaScript interop errors with prerendering. Blazor Server prerenders pages on the server before JavaScript is available, but localStorage requires JavaScript. I fixed this by disabling prerendering on interactive components."

> "Third, I had port configuration mismatches where hardcoded values didn't match the actual running ports. I resolved this by using configuration files properly."

> "And finally, there was a JWT claims bug where the task assignment dropdown wasn't showing the current user because I was looking for the wrong claim name - 'nameid' instead of 'sub'."

**[Show working application again]**

> "All these bugs are now fixed, and the application works smoothly as you've seen in this demonstration."

---

## 🎯 CONCLUSION (1 minute)

### What Was Achieved [11:00-11:30]

**[Show browser with application running]**

> "To summarize, I've built a complete full-stack task management application with:"

> "A RESTful API backend using .NET 8 with JWT authentication, Entity Framework Core for data access, and comprehensive service layer architecture."

> "A Blazor Server frontend with complete authentication, full CRUD operations for projects and tasks, filtering and search capabilities, and reusable UI components."

> "And a robust test suite with 74 unit tests achieving 95% code coverage, testing all scenarios including edge cases, validation, authorization, and business logic."

### Future Improvements [11:30-11:50]

**[Optional: Show Reflection.md section on future work]**

> "If I were to continue this project, I would add real-time collaboration using SignalR so team members see updates instantly, task dependencies and file attachments, a mobile app using Blazor Hybrid, and production-ready infrastructure with Docker and CI/CD pipelines."

### Closing [11:50-12:00]

**[Show project in VS Code or browser]**

> "This project demonstrates enterprise-grade web development patterns including authentication, authorization, clean architecture, comprehensive testing, and modern frontend framework usage. Thank you for watching!"

**[End recording]**

---

## 📝 RECORDING TIPS

### Before Recording:
1. ✅ Close unnecessary applications and browser tabs
2. ✅ Clear browser cache and localStorage
3. ✅ Test both API and Blazor startup
4. ✅ Prepare test data (or use seed data)
5. ✅ Set browser zoom to 100% for clarity
6. ✅ Run tests once to ensure all pass
7. ✅ Check audio levels
8. ✅ Use 1920x1080 resolution if possible

### During Recording:
1. ✅ Speak clearly and at moderate pace
2. ✅ Wait for UI transitions (loading spinners) to complete
3. ✅ Point cursor at relevant UI elements while speaking
4. ✅ Show success messages and notifications
5. ✅ If something fails, restart that section (edit in post)
6. ✅ Keep terminal output visible when running commands

### Screen Layout Suggestions:
- **Authentication & UI Demo**: Browser full screen or with minimal VS Code sidebar
- **Code Walkthrough**: VS Code full screen or split with browser
- **Testing**: Terminal in center with VS Code test files visible

### Time Management:
- Introduction: 1 min
- Running Application: 0.5 min
- Authentication: 2 min
- Projects: 2.5 min
- Tasks: 2.5 min
- Testing: 2 min
- Code Quality: 1 min
- Challenges: 1 min
- Conclusion: 1 min
- **Total: ~11-12 minutes**

### Pacing Strategy:
- Speak slightly faster during navigation/transitions
- Slow down when explaining technical concepts
- Pause briefly after each major feature demonstration
- Allow time for viewers to see success messages

---

## 🎬 ALTERNATIVE SHORTER VERSION (10 min exactly)

If you need to cut to exactly 10 minutes:

**Reduce:**
- Authentication (1:30 instead of 2 min) - Skip logout/login, just show registration
- Projects (2 min instead of 2.5) - Skip edit or delete
- Tasks (2 min instead of 2.5) - Skip edit or assignment
- Challenges (0:30 instead of 1 min) - Mention only top 2 bugs

**Keep full time for:**
- Testing demonstration (2 min) - This is critical for Part 2
- Creating new project and task (show CRUD works)
- Filtering demonstration (shows complexity)

---

## 📌 CRITICAL SECTIONS (Don't Skip)

1. ✅ Authentication working (login/register)
2. ✅ Creating a project (POST request)
3. ✅ Creating a task (POST request)
4. ✅ Filtering tasks (demonstrates frontend logic)
5. ✅ Running all 74 tests and showing they pass
6. ✅ Explaining test coverage (~95%)
7. ✅ Showing project structure (demonstrates organization)

These sections directly address the assignment requirements:
- "Blazor App Functionality" ✅
- "Testing the API" ✅
- "Document... implementation" ✅

---

**Good luck with your recording! 🎥**