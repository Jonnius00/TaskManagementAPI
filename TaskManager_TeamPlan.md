**Task Management API – Team Plan & Setup (Part 1)**

![Architecture outline](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABsAAAAVCAYAAAC33pUlAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAINJREFUSInVlOEKwCAIhM+x939l92MEIaUeOWH3J1C7DysDXima1AbqhLY2pVcn7TewT+5gZapOjvEJTW0sC0zVZUAZI7fGJnfGWSAF8zZkgNScKQDZ5CQAAQBuAraCD9C8bnUyZ6F5JYwWe4zzvYgTs/XhKVR/R72/vtdedWf0gzrSAywyKfcIP6TDAAAAAElFTkSuQmCC) This document outlines how a team of three will build a .NET 8 Task Management API with JWT authentication, Entity Framework Core (SQLite), clean architecture (Controllers, Services, Models, Data), and controller tests.
## **1. Roles and Responsibilities (3-Person Split)**
![Database outline](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAaCAYAAAC3g3x9AAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAIpJREFUSInllEsOwCAIRKHp/a9MVxqQTxDpytk0Kc5LHCYiaJHxzxN6A9oEhd4qyALD0wSbejkdgkwCiRsO4ArKxoDMSxxoHSypPcMOoIinAyjiGcDTLk4/3zKHZpaynidrkDGHZ7IZpmt0eW1OqmPWRg0TQvZ1H4fywwBw9ZZ/UaU6yhNtNIK7vg+DmiApZQy5xQAAAABJRU5ErkJggg==) **Student A - Evgenii** is a DB Lead responsible for Database & Models.\
• Owns: Models/, Data/TaskDbContext.cs, migrations, seeding, connection string.\
• Deliverables: User, Project, TaskItem (optional Comment) models; TaskDbContext with DbSets and relationships; seed sample data; SQLite wiring in Program.cs; first migration applied.\
• Contract: Model shapes stable; DbContext name = TaskDbContext; seeded users (e.g., alice Id=1, bob Id=2) with known password.\
\
![Download from cloud outline](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB8AAAAYCAYAAAACqyaBAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAIZJREFUSInNVcEWgCAIm/3/P9utZ0QWQ8DdUGEI6Br86MJuC2KaCOXebD+M2HTuT4nSy/pGnOa/om9UjJUDEzqAFnwmEZ2lGp8hZRPtAHAMRtoTemRR4HvdvARV5Leel8I1tSy2uDnAfYMWeXVVSXOWa6G/44wsRTw0wlTVGklL5HIbnaZwAv0KLOPA0jfVAAAAAElFTkSuQmCC)  **Student B - Vu Hoc** is a head of Controllers & Services (Projects/Tasks/Comments).\
• Owns: Controllers for Project/Task (optional Comment), Services for Project/Task, DTOs.\
• Rules: Controllers thin; business logic in Services; enforce ownership using IUserContext.\
• Unit tests: happy paths, 404 (non-owner), invalid cases.\
\
` `![Shield Tick outline](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABoAAAAfCAYAAAD5h919AAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAALRJREFUSInFllEOgDAIQzfvf2f9MUbRjrYOJfFHoG8DcWtNt3V/yiyKl8CQ6FQYI5bG9CQx+s+CyIc0j6CsyUqPHvWybasLgP5Ro9lmMxW5OGfMCNSIoBJbqoT/AK0syCnnLYcBdRNmmzvMME8dzuwd/DvQky6AafGneOboKBtcmIe+uuGv/i00Op1yWiAFZuU5SXYl1NOUjbUFplzBRsLyoDIwZ2evgOxCpsM+uX9/ej5JtgHHMWKu1ucQ1QAAAABJRU5ErkJggg==) **Student C - Ayush** is a security boss, takes control of Auth (JWT) & Users\
• Owns: AuthController (register/login), TokenService, JWT wiring in Program.cs, Swagger JWT.\
• Token claims: sub = user id, unique\_name = username.\
• Definition of Done: protected endpoints require Bearer token; unauthorized yields 401.
## **2. ![Plug outline](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAZCAYAAAA4/K6pAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAHxJREFUOI21VEEOwCAIc8b/f5mdtgipQGHrRYO0FoKO4UOC83ElyV6eS7YrjZC4QBJld24kus7HAQvV3AWCEaDTkwCKq9gECRkHr8hEQQa7g5KILaHsxMIb4fQFApJpd3LY0wKt19hqakiuTKLCb1+aJ6ZwKuHTaWwJhsQbMAwjBW8aTnYAAAAASUVORK5CYII=) Repository & Branching**
Initial setup (one person):\
`  `mkdir C:\task-mgr && cd C:\task-mgr\
`  `dotnet new sln -n TaskManager\
`  `dotnet new webapi -n TaskManager.Api --use-controllers\
`  `dotnet new xunit -n TaskManager.Tests\
`  `dotnet sln add .\TaskManager.Api\TaskManager.Api.csproj .\TaskManager.Tests\TaskManager.Tests.csproj\
`  `git init && git add . && git commit -m "chore: solution bootstrap"\
\
Branches: main (protected), feat/db (A), feat/projects-tasks (B), feat/auth (C). PRs must build & pass tests.
## **3. ![Server outline](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABYAAAAVCAYAAABCIB6VAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAGFJREFUOI3tlFELABAMhG/+/3/mQWSxbHWK8j1RczubEVQyeMi4YQoDAFIwPjvW5gEKUcc7ukG2sJ2JxXvNa4/5+gGZdH7zVKFZ4rIP0YQSHxtpj+2V0/B1PcKukhz/3egUTl4Y/goOTXsAAAAASUVORK5CYII=) Data Model (Owned by Student A)**
Models:\
• User: Id, Username, Email, PasswordHash, Projects, AssignedTasks, Comments?\
• Project: Id, Name, Description, CreatedDate, UserId (owner), Tasks\
• TaskItem: Id, Title, Description, Status (ToDo/InProgress/Done), DueDate, ProjectId, AssignedUserId, Comments?\
• Comment (optional): Id, Text, CreatedDate, TaskItemId, UserId\
\
DbContext (TaskDbContext): DbSet<User>, DbSet<Project>, DbSet<TaskItem>, DbSet<Comment>; HasData() seeding for users/projects/tasks.\
SQLite connection string in appsettings.json: ConnectionStrings:Default = "Data Source=tasks.db".\
Migrations: dotnet ef migrations add InitialCreate && dotnet ef database update.
## **4. ![Full battery outline](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAANCAYAAACzbK7QAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAADhJREFUOI1jZGBg+M9AfcCIzKGFBXDARED+Pw4an1qSLKAY0NqC/0PeB0PfAkaa+4CBYYAz2uAHAKQiCA6EEm+RAAAAAElFTkSuQmCC) Program.cs Wiring (Owned by A & C)**
• EF Core: AddDbContext<TaskDbContext>(UseSqlite(...))\
• CORS: AddCors("frontend") allowing any origin/headers/methods (dev)\
• Authentication/Authorization: AddAuthentication(JwtBearer) + AddAuthorization\
• Swagger with JWT security definition\
• MapControllers(); optional root redirect to /swagger
## **5. ![Blind outline](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABsAAAAZCAYAAADAHFVeAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAI5JREFUSIntlMEOgCAMQ6v//8/zpCGE0RaSXWQnlJXXLAWgpqKI8wfQtSOenBPm2Z8omrViwh6dApnpZE0ka0dHtaNNd3QSUAU5htr9aD8UA7vwuImrWb3xXkof+7canGG/miYXmvYwILtH9gXPnCup601J1Tc749p66dXRUQh7mdV42y/8CFRSB3RAAIAHGkVRuAesq3oAAAAASUVORK5CYII=) Auth Layer (Owned by Student C)**
• DTOs: RegisterDto(username,email,password), LoginDto(username,password), AuthResultDto(token, userId,username)\
• TokenService: creates JWT with sub and unique\_name claims\
• AuthController: /api/auth/register and /api/auth/login; hashes passwords with BCrypt; returns token & user info\
• Swagger: Authorize button with Bearer token
## **6. ![Decision chart outline](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAAaCAYAAACtv5zzAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAIZJREFUSIntVcsOwCAIg2X//8vssoNBgRIki8l60QhtfaAy+ZAgTkTEQE7JwMVVFfjc4EcbytXlibaIa5NzIdS4CjH6LlYPlUXWuWgeZKDHLHFoZRHZEykZIAJTPPuapj+X0QDd5xVMrlcZ2dnC3MpFmrgMClozKldNFA+553/699tuPdgRDwViJvXCSUuMAAAAAElFTkSuQmCC)  Controllers & Services (Owned by Student B)**
Abstraction for current user:\
• IUserContext { int UserId { get; } } and HttpUserContext reading claim "sub"\
• Register IHttpContextAccessor & IUserContext in DI; tests can use FakeUserContext\
\
ProjectController (Authorize): CRUD; only owner can read/update/delete\
TaskController (Authorize): CRUD; UpdateStatus; Assign; only owner’s projects’ tasks accessible\
Optional CommentController: CRUD on task comments\
\
Services (ProjectService/TaskService): implement queries with owner checks and map to DTOs.
## **7. ![Internet Of Things outline](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB0AAAAaCAYAAABLlle3AAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAJJJREFUSInFVkEOwCAIQ+P/v+wuHowTECvQxItR2q6ESWRHH+saFbgLEYcTIk41sDGspHBeJ9CcIgLKWKFgBVuVzIVCXOyUu+YvFe8U1ICSABWVElS+Hg6FeBO//fWQh/MQUrVGaufNnwBpMPGeZaqcCng6qdIicMeLPLd94fWnWImOeFxHI/oavBLWmP3wZ4Y7Pv2PMOz0HjIiAAAAAElFTkSuQmCC) ![Network outline](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB0AAAAbCAYAAACAyoQSAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAKVJREFUSInVVtESgCAIs/7/n+mlF5GBINC1O6885yYq1Bgx0NvawM3KzSWD1ohbTQm8txj/a1vvzFV4TYm1TBB7rp2AmIdDaCAquI3omYpbJnDU6oXO1OpzIaQx8S5lpUhYmkPK2BGsrdzGJ3nqBYqotCTuXKx0Q8281PCEk2ro4mo5p3E8xub8jJ8uU6MjT5dIu4rDFG2VKU+nKVrrIp0UcPgReAC/wkjRvv5uoAAAAABJRU5ErkJggg==) Endpoints (Contracts)**
Auth:\
• POST /api/auth/register  { username, email, password } → { token, userId, username }\
• POST /api/auth/login     { username, password } → { token, userId, username }\
\
Projects:\
• GET /api/project?search=\
• POST /api/project { name, description }\
• GET /api/project/{id}\
• PUT /api/project/{id} { name, description }\
• DELETE /api/project/{id}\
\
Tasks:\
• GET /api/task?projectId=&status=\
• POST /api/task { title, description, status, dueDate, projectId, assignedUserId }\
• GET /api/task/{id}\
• PUT /api/task/{id} { title, description, status, dueDate, projectId, assignedUserId }\
• PATCH /api/task/{id}/status { status }\
• PATCH /api/task/{id}/assign { assignedUserId }\
• DELETE /api/task/{id}
## **8. ![Circles with arrows outline](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABUAAAAXCAYAAADk3wSdAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAIBJREFUOI3NVEEOwCAIq8v+/2V2WuKYVBC22JMKFC1IA4d06zbxdUHmLpskYSQ0gdZJ1DnTVPtSRJ/58j8SZPFsi/ECPG+aIbtR0ss9+S99XYsKDcrnA0uwLLQZow2ZKgoAnAmC0UWaPrD2HjLTONLVCqI1YC0QGYNufPbt9vnPFxhcJu8NclsKAAAAAElFTkSuQmCC) Testing & Coverage**
• Each student writes controller tests for their area (happy paths, 401/404/invalid)\
• Use in-memory SQLite; FakeUserContext(UserId=1) for ownership\
• Run tests: dotnet test\
• Coverage (controllers-only):\
`  `dotnet test .\TaskManager.Tests\TaskManager.Tests.csproj --collect "XPlat Code Coverage" --results-directory ".\TestResults"\
`  `reportgenerator -reports:".\TestResults\\*\*\coverage.cobertura.xml" -targetdir:".\TestReport.ControllersOnly" -reporttypes:Html -assemblyfilters:+TaskManager.Api -classfilters:+TaskManager.Api.Controllers.\*
## **9.**  ![Daily calendar outline](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABoAAAAaCAYAAACpSkzOAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAGJJREFUSIljYMAO/uMQJwRw6mMi00CSASM+V1DZHqwWDd2gw+sKaoKRkxioDugWdCxofGr6jhFd4D8ONjEOINphdAs6uidvWHiOxhHRYDSOiBXHAMMvjhgYhlvpTdegowsAALYOGw2jZDVkAAAAAElFTkSuQmCC) **Work Process Notes (for the write-up)** 
• What worked: parallel work via clear ownership; IUserContext abstraction; seeds enabling quick manual tests\
• Challenges: ownership checks across joins; JWT wiring and Swagger Bearer setup; migration conflicts\
• Do differently: add paging/sorting early; add CI workflow to run tests on PR; add DTO annotations for validation\
• Extras (optional): comments, notifications on status change/assignment, file URLs on tasks, sorting & filtering.
## **10. ![Explosion outline](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB4AAAAaCAYAAACgoey0AAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAJVJREFUSInNlVkOgDAIRKv3vzP+aGJUKDMsdRLjguUBLe0YvOS8WiWPeytUe28DL4UuCYaC71WOs+SFuoNDMqYh2YDQQotkrGnLcGJFz9rCmu1gNPxrIDKvFBgFpO1oojxroJQ59ZQtXHJvO10tgpb6PtaUFbFVfu2bxwYHhP6n2mbOS+xdR96L8+uzNp23JFurP0uZBxv4QM2wuNLnAAAAAElFTkSuQmCC) Risks & Mitigations**   
• Schema churn → DB lead as single owner of migrations/seed; announce changes via PR\
• Auth coupling → Controllers depend on IUserContext only; tests use FakeUserContext\
• Permissions bugs → Centralize owner filtering in Services; add negative (non-owner) tests\
• Merge conflicts → Branch per feature; small PRs; code reviews
