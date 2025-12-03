# FitnessTracker – CSCI 3110 Term Project (Fall 2025)

FitnessTracker is an ASP.NET Core MVC web application that helps users log their workouts, track progress over time, and collaborate with trainers. The project was built as the term project for CSCI 3110 to demonstrate a full-stack MVC application with a REST Web API, Entity Framework Core data model, and JavaScript/AJAX-driven features.

---

## Table of Contents

1. [Project Proposal](#project-proposal)
2. [Main Features](#main-features)
   - [Regular User Features](#regular-user-features)
   - [Trainer Features](#trainer-features)
3. [Data Model Overview](#data-model-overview)
4. [Architecture & Technologies](#architecture--technologies)
5. [REST Web API Endpoints](#rest-web-api-endpoints)
6. [JavaScript & AJAX Features](#javascript--ajax-features)
7. [Accessibility Principles](#accessibility-principles)
8. [How to Run the Application](#how-to-run-the-application)
9. [AI Use & Disclosure](#ai-use--disclosure)

---

## Project Proposal

### Idea

FitnessTracker is a lightweight, Gymshark-inspired fitness dashboard that allows:

- **Regular users** to log workouts, track weight/body-fat changes, and see their progress in charts.
- **Trainers** to request access to clients, assign workout templates, and view client progress with feedback.

The goal is to create something that feels like a simplified version of a modern fitness platform while still being small enough for a semester project.

### Target Users

- **Regular users / gym members**
  - Want a simple way to log workouts and visualize progress.
- **Trainers**
  - Need a way to manage clients, assign structured workout plans, and give feedback on progress.

### Core User Stories (Summary)

- As a **user**, I can create an account and sign in so that I have a personal dashboard.
- As a **user**, I can create, edit, and delete my workouts.
- As a **user**, I can log my weight (and optional body fat) and see my progress on a chart.
- As a **trainer**, I can request access to a client’s data.
- As a **user**, I can accept or decline trainer access requests.
- As a **trainer**, once a request is accepted, I can view that client’s progress and add feedback.
- As a **trainer**, I can create workout templates and assign them to clients.
- As a **user**, I can see workouts assigned by my trainer and log my sets/reps/weights for each exercise.

### Planned Technologies

- **Backend:** ASP.NET Core MVC, C#, Entity Framework Core
- **Database:** SQLite (via EF Core)
- **Frontend:** Razor views, Bootstrap 5, custom CSS
- **API:** ASP.NET Core REST Web API controllers
- **Client-side:** JavaScript (ES modules), Fetch API, Chart.js for charts
- **Version control:** Git + GitHub

---

## Main Features

### Regular User Features

- **Authentication & roles**
  - Register, login, logout.
  - Role stored in session (`User` or `Trainer`).
- **Home page**
  - Marketing-style landing page with hero section, feature highlights, and calls to action.
- **User dashboard**
  - Personalized welcome message using the session name.
  - Quick access cards for:
    - **Workouts**
    - **Progress**
    - Trainer access requests (if any pending).
- **Workout Hub**
  - Add, edit, and delete workouts via a REST API and AJAX.
  - Fields include workout style/name, duration, total sets, total reps, and notes.
  - Workouts are displayed as responsive cards.
- **Assigned Workouts**
  - “Assigned Workouts” section shows workouts assigned by a trainer.
  - Clicking “Use this workout” opens a page where the user logs set-by-set data for each exercise in the template.
- **Progress Tracking**
  - Log entries with:
    - Date
    - Weight (required)
    - Body fat percentage (optional)
    - Notes (optional)
  - Progress view:
    - Chart.js line chart for weight and body fat.
    - History table listing all entries.
    - Edit and delete operations for each entry.

### Trainer Features

- **Trainer Dashboard**
  - Overview entry point for trainer-specific actions.
- **Client Management**
  - List of all users that can become clients.
  - Trainer can:
    - Send an **access request** to a user.
    - See status: Pending, Accepted, Declined.
    - If accepted, view a client’s progress.
- **Access Request Workflow**
  - Trainer sends a request.
  - User sees the request on their dashboard and can Accept/Decline.
  - Status is reflected on the trainer’s Clients page.
- **View Client Progress**
  - Trainer can open a client’s progress page:
    - See a weight chart.
    - View each log entry.
    - Add trainer feedback to individual progress entries via a reply form.
- **Workout Templates & Assignments**
  - Global templates are seeded (Biceps, Triceps, Back, Legs) with common exercises.
  - Trainers see and use these templates when assigning workouts.
  - Trainer assigns a template to a client on a specific date.
  - The assignment appears on the client’s “Assigned Workouts” page.

---

## Data Model Overview

Entity classes (simplified):

- `User`
  - `UserId`, `Name`, `Email`, `PasswordHash`, `Role`
  - Navigation: `Workouts`, `SavedTemplates`, `AssignedWorkouts`
- `Workout`
  - `WorkoutId`, `UserId`, `Date`, `WorkoutStyle`, `DurationMinutes`, `TotalSets`, `TotalReps`, `Notes`
  - Navigation: `User`, `Sets`
- `WorkoutSet`
  - Per-set logging for assigned workouts:
  - `WorkoutSetId`, `WorkoutId`, `ExerciseName`, `SetNumber`, `Reps`, `Weight`
- `WorkoutTemplate`
  - `WorkoutTemplateId`, `TemplateName`, `Category`
  - Navigation: `Exercises`
- `WorkoutTemplateExercise`
  - `WorkoutTemplateExerciseId`, `WorkoutTemplateId`, `ExerciseName`, `Sets`, `Reps`, `Weight`
- `UserWorkoutTemplate` & `UserWorkoutTemplateExercise`
  - Personal templates owned by a trainer or user.
- `ProgressLog`
  - `ProgressLogId`, `UserId`, `EntryDate`, `Weight`, `BodyFatPercent`, `Notes`, `TrainerFeedback`
- `TrainerClientRequest`
  - `TrainerClientRequestId`, `TrainerId`, `ClientId`, `Status`, `SentDate`
- `TrainerAssignedWorkout`
  - `TrainerAssignedWorkoutId`, `TrainerId`, `ClientId`, `AssignedDate`, `WorkoutTemplateId`, `IsCompleted`, `CompletedDate`

### Many-to-Many Relationship

The project includes a many-to-many relationship between **Trainers** and **Clients**, modeled via join entities:

- `TrainerClientRequest` (Trainer ↔ Client, for permissions)
- `TrainerAssignedWorkout` (Trainer ↔ Client, for plans)

Each trainer can have many clients, and each client can have relationships with more than one trainer in principle, satisfying the requirement for at least one many-to-many.

---

## Architecture & Technologies

- **Framework:** ASP.NET Core MVC (.NET 9)
- **ORM:** Entity Framework Core with SQLite
- **Pattern:**
  - Traditional MVC controllers for views
  - Separate **API controllers** for JSON-based operations
- **Client-side:**
  - JavaScript ES modules (`WorkoutAJAXRepository`, `TrainerRequestAJAXRepository`, `WorkoutDOM`, etc.)
  - Fetch API for AJAX calls.
  - Chart.js for progress graphs.
- **Styling:**
  - Bootstrap 5 for base layout and components.
  - Custom CSS classes (e.g., `ft-hero`, `ft-feature-card`, `ft-btn-black`) for branding.

---

## REST Web API Endpoints

### `/api/workout`

- `GET /api/workout/all`  
  Returns all workouts for the currently logged-in user.
- `GET /api/workout/one/{id}`  
  Returns one workout by ID (only if it belongs to the current user).
- `POST /api/workout/create`  
  Creates a new workout using form data.
- `PUT /api/workout/update`  
  Updates an existing workout identified by `WorkoutId` in the form data.
- `DELETE /api/workout/delete/{id}`  
  Deletes a workout for the current user.
- `GET /api/workout/sources`  
  Returns trainer plan for today (if any), personal templates, and global templates.
- `GET /api/workout/template-details/{sourceType}/{templateId}`  
  Returns detailed template information with exercises and totals.
- `GET /api/workout/assigned/{assignedId}`  
  Returns summary data for an assigned trainer workout for pre-filling the form.

### `/api/trainerrequest`  (summary)

- `GET /api/trainerrequest/mine`  
  Returns pending trainer requests for the currently logged-in user.
- `POST /api/trainerrequest/respond`  
  Accepts or declines a trainer request (based on `RequestId` and `Decision` form data).

---

## JavaScript & AJAX Features

1. **Workout Hub (WorkoutIndex)**
   - File: `wwwroot/js/workoutIndex.js`
   - Uses:
     - `WorkoutAJAXRepository` to call `/api/workout`.
     - `WorkoutDOM` for rendering cards, showing alerts, spinners, and handling form state.
   - Features:
     - Load all workouts on page load.
     - Add, edit, and delete workouts without reloading the page.
     - Populates the edit form when “Edit” is clicked.

2. **Trainer Requests**
   - File: `TrainerRequestAJAXRepository.js` + integration in `workoutIndex.js` and `WorkoutDOM`.
   - Features:
     - Load current user’s trainer requests via AJAX.
     - Show requests near the workout area.
     - Accept/decline requests asynchronously and update the UI.

3. **Assigned Workout Logging**
   - File: `assignedEntry.js`
   - Features:
     - Reads JSON from `ExerciseData` hidden input.
     - Dynamically builds set-by-set input fields for each exercise (reps and weight).
     - Ensures numeric input only for sets/reps/weights.

4. **Charts**
   - Progress view uses Chart.js to render a line chart for weight and optional body fat percentage, using JSON data serialized from the model.

---

## Accessibility Principles

The application attempts to follow **WCAG 2.1 AA**-aligned practices at a basic level:

- **Semantic structure**
  - Proper use of headings (`h1`, `h2`, etc.) and sectioning elements.
  - Labels associated with form inputs using `<label>` and Razor tag helpers (`asp-for`).
- **Keyboard operability**
  - All interactive elements (links, buttons, forms) are standard HTML controls usable with keyboard navigation.
- **Color contrast**
  - Dark text on light backgrounds and light text on dark backgrounds for hero, cards, and buttons.
- **Form feedback**
  - Validation messages are displayed near their corresponding fields.
  - Inline alerts (`alert` classes) provide feedback for success/error messages.
- **Responsive design**
  - Layout uses Bootstrap’s grid to adapt to different screen sizes.

This is not a full audit, but accessibility was considered when structuring the UI and forms.

---

## How to Run the Application

1. **Prerequisites**
   - .NET 9 SDK installed
   - Git installed

2. **Clone the repository**

   ```bash
   git clone <your-repo-url>.git
   cd FitnessTracker
