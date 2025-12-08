# Personal Reflection: Building a Full-Stack Task Management Application

## Overview

This assignment challenged me to build a complete Blazor Server frontend for the Task Management API I developed in Part 1. The goal was to create an interactive web application with authentication, full CRUD operations for projects and tasks, and comprehensive testing—all while maintaining clean architecture and usability. Looking back, this project taught me valuable lessons about real-world web development, debugging, and the importance of systematic problem-solving.

---

## My Approach: Planning vs. Execution

### Initial Planning

I started by creating a detailed implementation plan (see `IMPLEMENTATION_PLAN_PART2.md`), breaking the work into 8 stages:
1. Project setup and configuration
2. Authentication module
3. Project management CRUD
4. Task management CRUD
5. UI/UX polish
6. Comprehensive API testing
7. Documentation
8. Delivery preparation

This structured approach helped me stay organized, though I quickly learned that **planning and reality don't always align perfectly**. The plan estimated certain tasks would take 2-3 days, but unexpected technical challenges pushed some stages longer while others completed faster than expected.

### Key Architectural Decisions

Early on, I made three critical technology choices that shaped the entire project:

**1. Blazor Server over WebAssembly**  
I chose Blazor Server for its simplicity and fast development cycle. Since this was a student assignment with a small user base, I didn't need the scalability of WebAssembly. The server-side execution also simplified debugging—I could use Visual Studio's debugger directly on the C# code handling UI logic.

**2. HttpClient in C# over JavaScript**  
I decided to use C#'s `HttpClient` for API communication rather than JavaScript interop. This kept the entire application in one language, provided type safety end-to-end, and integrated naturally with dependency injection.

**3. localStorage for JWT Tokens**  
For JWT token persistence, I chose `localStorage` over `sessionStorage` to maintain user sessions across browser restarts. While this isn't the most secure approach for production apps (vulnerable to XSS), it was appropriate for this educational project and provided better UX during development and testing.

---

## What Was Challenging

### The JWT Token Persistence Mystery (Critical Bug #1)

The most frustrating challenge came after implementing authentication. Users could log in successfully, the JWT token was stored in `localStorage`, but every subsequent API call returned **401 Unauthorized**. I spent hours debugging, checking:
- Was the token being saved? Yes.
- Was it in the correct format? Yes.
- Was the API receiving it? No!

**The problem**: I initially registered `HttpClient` with `AddScoped<HttpClient>()` and set the Authorization header once after login. However, Blazor Server creates new `HttpClient` instances per scope, so each API call got a *new* client without the token.

**The solution**: I refactored `ApiClient` to retrieve the JWT token from `localStorage` on **every request** rather than storing it in the `HttpClient` headers. This pattern, while having slight overhead, proved reliable and solved the persistence issue completely.

### The Prerendering Nightmare (Critical Bug #2)

After fixing authentication, I hit another wall: navigating to project or task detail pages threw JavaScript interop errors: *"JavaScript interop calls cannot be issued at this time."*

**The problem**: Blazor Server prerendering executes C# code on the server *before* JavaScript is available in the browser. My components were trying to access `localStorage` (via Blazored.LocalStorage library) during prerendering, which requires JavaScript.

**The solution**: I disabled prerendering on all interactive pages by adding:
```csharp
@rendermode @(new InteractiveServerRenderMode(prerender: false))
```

This was a trade-off—pages now have a slight delay before becoming interactive—but it was necessary for `localStorage` to function properly.

### Port Configuration Confusion (Critical Bug #3)

Midway through development, I discovered all my API calls were failing with connection errors. I had hardcoded the API URL as `http://localhost:5001`, but the API was actually running on port `5114` (defined in `launchSettings.json`).

**The fix**: I created an `ApiSettings` section in `appsettings.json` and updated all configuration files to use the correct port. This taught me to always read configuration from settings files rather than hardcoding values.

### Task Assignment JWT Claims Bug (Critical Bug #4)

When implementing task assignment, the dropdown only showed "Unassigned" instead of "Me (@username)". After debugging, I discovered my `GetCurrentUserIdAsync()` method was looking for a `"nameid"` claim, but my JWT token used `"sub"` (the standard claim name).

**The fix**: Updated the claim lookup to check for `"sub"` first with a fallback to `ClaimTypes.NameIdentifier`. This highlighted the importance of understanding JWT standards and not making assumptions about token structure.

### Test Expansion Complexity

Growing the test suite from 44 to 74 tests required careful planning. I needed to:
- Add a third test user (charlie) for empty-list scenarios
- Implement manual `ModelState` validation in tests (since unit tests don't run ASP.NET's automatic validation)
- Test cascade deletes, status transitions, combined filters, and authorization boundaries

The testing process was methodical but time-consuming, taking an entire session to expand coverage from ~70% to ~95% across all controllers.

---

## What Went Well

### Component-Based Architecture

Blazor's component model worked beautifully. Breaking the UI into small, reusable pieces (`LoadingSpinner`, `ErrorAlert`, `ConfirmDialog`) made the code maintainable and consistent. I could build complex pages by composing simple components, much like React but without leaving C#.

### Service Layer Separation

Following the service layer pattern paid off. Having `ProjectApiService` and `TaskApiService` centralize all API communication made the codebase clean. When I needed to change how tokens were handled, I only modified `ApiClient`—all services continued working without changes.

### Achieving High Test Coverage

Reaching 74 tests with ~95% controller coverage felt like a significant achievement. The systematic approach (CRUD operations, validation, authorization, edge cases) ensured I tested all critical paths. Seeing that green "✅ All tests passing" message was deeply satisfying.

### Responsive UI with Bootstrap

Using Bootstrap 5 made the UI look professional with minimal effort. The responsive grid system worked out-of-the-box, and the application looked good on both desktop and mobile without custom CSS.

### Problem-Solving Process

Each bug forced me to dig deeper into how Blazor, HTTP clients, and JWT tokens actually work under the hood. The debugging process—while frustrating—taught me more than any tutorial could. I learned to:
- Read error messages carefully
- Check assumptions systematically
- Consult official documentation
- Test hypotheses methodically

---

## How I Would Continue Working on This Application

If I were to continue developing this application, I would focus on these key improvements:

### 1. Real-Time Collaboration Features
Implement SignalR for live updates when team members create or modify tasks. Users could see changes instantly without refreshing, making it feel like a true collaborative tool (similar to Trello or Notion).

### 2. Enhanced Task Management
- **Task dependencies**: Prevent Task B from starting until Task A is complete
- **Due date notifications**: Email reminders for approaching deadlines
- **File attachments**: Allow users to upload documents, images, or screenshots to tasks
- **Activity log**: Track who changed what and when (audit trail)

### 3. Mobile Application
Build a Blazor Hybrid mobile app (using .NET MAUI) that shares the same business logic but provides a native mobile experience. This would demonstrate code reuse across platforms—one C# codebase serving web and mobile users.

### 4. Production-Ready Infrastructure
- Migrate from SQLite to PostgreSQL for better concurrency
- Implement Docker containerization for consistent deployment
- Add CI/CD pipeline (GitHub Actions) to run tests automatically on every commit
- Set up proper logging and monitoring (Application Insights or Sentry)

### 5. Performance Optimizations
- Add paging for project and task lists (currently loads everything)
- Implement caching for frequently accessed data
- Optimize database queries with eager loading for navigation properties

---

## Conclusion

This assignment pushed me far beyond my comfort zone. I encountered real-world problems that don't appear in tutorials: JWT token persistence across scopes, JavaScript interop timing issues, and the subtle complexities of Blazor's rendering lifecycle.

The most valuable lesson was learning to embrace debugging as part of the development process. Every bug I fixed deepened my understanding of how web applications actually work—not just how they're supposed to work according to documentation.

Building a full-stack application from scratch (API + frontend + tests) gave me confidence that I can tackle complex software projects independently. The systematic approach—plan, implement, test, debug, document—proved effective and will serve me well in future development work.

**Final Stats**:
- ~9,000 lines of C# code
- 74 unit tests (95% coverage)
- 19 Blazor components
- 4 critical bugs fixed
- 1 fully functional task management system

Would I do anything differently? Absolutely—I'd start with better port configuration, understand Blazor's prerendering from day one, and test token persistence earlier. But those mistakes taught me lessons I won't forget.

---
---

## Bugfixes notated during the development process

### �🔧 Critical Bug Fixes Applied (December 7, 2025)

**1. JWT Token Persistence Issue (RESOLVED)**
- **Issue**: 401 Unauthorized errors after successful login
- **Root Cause**: HttpClient instances per scope didn't share token in DefaultRequestHeaders
- **Solution**: Refactored ApiClient to retrieve token from localStorage on every request
- **Files Modified**: `ApiClient.cs`, `Program.cs`, `AuthenticationService.cs`

**2. JavaScript Interop Prerendering Issue (RESOLVED)**
- **Issue**: "JavaScript interop calls cannot be issued" error on navigation
- **Root Cause**: Blazored.LocalStorage requires JavaScript, unavailable during server prerendering
- **Solution**: Added `@rendermode @(new InteractiveServerRenderMode(prerender: false))` to all page components
- **Files Modified**: All 6 project/task CRUD components

**3. Configuration Mismatch (RESOLVED)**
- **Issue**: API endpoint mismatch (configured for 5001, actual 5114)
- **Solution**: Updated all appsettings.json files to use correct port
- **Files Modified**: Blazor and API appsettings.json files

**4. Task Assignment Feature - JWT Token Claim Bug (RESOLVED)**
- **Issue**: Task assignment dropdown only showed "Unassigned", missing current user option
- **Root Cause**: GetCurrentUserIdAsync() looked for "nameid" claim, but JWT token uses "sub" claim
- **Solution**: Updated claim lookup to search for "sub" (JwtRegisteredClaimNames.Sub) first, with fallback to full ClaimTypes.NameIdentifier
- **Files Modified**: `AuthenticationService.cs`, `CreateTaskComponent.razor`, `EditTaskComponent.razor`
- **Enhancement**: Added assignment dropdown to Create and Edit task forms

