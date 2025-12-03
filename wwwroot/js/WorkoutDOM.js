'use strict';

/**
 * Handles all DOM updates for the Workout page:
 * - Loading and empty states
 * - Alerts
 * - Workout cards
 * - Form population / reset
 * - Trainer request notifications
 *
 * This module does not talk to the server directly. It only manipulates
 * the HTML based on data provided by the AJAX layer.
 */
const WorkoutDOM = {

    /**
     * Shows the loading spinner on the workout list.
     */
    showLoading() {
        const el = document.getElementById("loadingSpinner");
        if (!el) {
            console.warn("[WorkoutDOM] showLoading: #loadingSpinner not found in DOM.");
            return;
        }

        console.info("[WorkoutDOM] Showing loading spinner.");
        el.style.display = "block";
    },

    /**
     * Hides the loading spinner on the workout list.
     */
    hideLoading() {
        const el = document.getElementById("loadingSpinner");
        if (!el) {
            console.warn("[WorkoutDOM] hideLoading: #loadingSpinner not found in DOM.");
            return;
        }

        console.info("[WorkoutDOM] Hiding loading spinner.");
        el.style.display = "none";
    },

    /**
     * Renders a Bootstrap alert message at the top of the workout section.
     *
     * @param {string} message  Text content of the alert.
     * @param {"info"|"success"|"warning"|"danger"} [type="info"]  Bootstrap alert style.
     */
    showAlert(message, type = "info") {
        const container = document.getElementById("alertContainer");
        if (!container) {
            console.warn("[WorkoutDOM] showAlert: #alertContainer not found. Message:", message);
            return;
        }

        console.info("[WorkoutDOM] Showing alert:", { type, message });

        container.innerHTML = `
            <div class="alert alert-${type} alert-dismissible fade show">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;
    },

    /**
     * Renders the list of workout cards based on the provided array.
     *
     * @param {Array<object>} workouts Array of workout objects from the API.
     */
    showWorkoutCards(workouts) {
        const container = document.getElementById("workoutCardsContainer");
        const emptyState = document.getElementById("emptyState");

        if (!container) {
            console.warn("[WorkoutDOM] showWorkoutCards: #workoutCardsContainer not found.");
            return;
        }

        console.group("[WorkoutDOM] showWorkoutCards");
        console.debug("[WorkoutDOM] Incoming workouts:", workouts);

        container.innerHTML = "";

        if (!Array.isArray(workouts) || workouts.length === 0) {
            console.info("[WorkoutDOM] No workouts to display. Showing empty state.");
            if (emptyState) emptyState.style.display = "block";
            console.groupEnd();
            return;
        }

        if (emptyState) emptyState.style.display = "none";

        workouts.forEach(w => {
            const card = document.createElement("div");
            card.className = "col";

            const title = w.workoutStyle ?? "(Untitled workout)";
            const sets = w.totalSets ?? 0;
            const reps = w.totalReps ?? 0;

            console.debug("[WorkoutDOM] Rendering workout card:", {
                workoutId: w.workoutId,
                workoutStyle: title,
                totalSets: sets,
                totalReps: reps
            });

            card.innerHTML = `
                <div class="card shadow-sm h-100">
                   <div class="card-body">
                       <h4>${title}</h4>
                       <p class="text-muted mb-1">Sets: ${sets}</p>
                       <p class="text-muted mb-3">Reps: ${reps}</p>

                       <button class="btn btn-dark edit-btn w-100 mb-2"
                          data-workout-id="${w.workoutId}">
                          Edit
                       </button>

                       <button class="btn btn-danger delete-btn w-100"
                          data-workout-id="${w.workoutId}">
                          Delete
                       </button>
                   </div>
                </div>
            `;

            container.appendChild(card);
        });

        console.info("[WorkoutDOM] Finished rendering", workouts.length, "workout card(s).");
        console.groupEnd();
    },

    /**
     * Populates the workout form with an existing workout’s data for editing.
     *
     * @param {object} workout Workout object from the API.
     */
    populateFormForEdit(workout) {
        console.group("[WorkoutDOM] populateFormForEdit");
        console.debug("[WorkoutDOM] Workout payload:", workout);

        const idEl = document.getElementById("WorkoutId");
        const styleEl = document.getElementById("WorkoutStyle");
        const setsEl = document.getElementById("TotalSets");
        const repsEl = document.getElementById("TotalReps");
        const notesEl = document.getElementById("Notes");
        const submitBtn = document.getElementById("submitBtn");
        const cancelBtn = document.getElementById("cancelBtn");

        if (!idEl || !styleEl || !setsEl || !repsEl || !notesEl || !submitBtn || !cancelBtn) {
            console.error("[WorkoutDOM] populateFormForEdit: One or more form elements are missing.");
            console.groupEnd();
            return;
        }

        idEl.value = workout.workoutId ?? 0;
        styleEl.value = workout.workoutStyle ?? "";
        setsEl.value = workout.totalSets ?? "";
        repsEl.value = workout.totalReps ?? "";
        notesEl.value = workout.notes ?? "";

        submitBtn.textContent = "Update Workout";
        cancelBtn.style.display = "inline-block";

        console.info("[WorkoutDOM] Form populated for editing workout:", workout.workoutId);
        console.groupEnd();
    },

    /**
     * Resets the workout form back to the "Add" state.
     */
    resetForm() {
        console.group("[WorkoutDOM] resetForm");

        const form = document.getElementById("workoutForm");
        const idEl = document.getElementById("WorkoutId");
        const submitBtn = document.getElementById("submitBtn");
        const cancelBtn = document.getElementById("cancelBtn");

        if (!form || !idEl || !submitBtn || !cancelBtn) {
            console.error("[WorkoutDOM] resetForm: One or more form elements are missing.");
            console.groupEnd();
            return;
        }

        form.reset();
        idEl.value = 0;
        submitBtn.textContent = "Add Workout";
        cancelBtn.style.display = "none";

        console.info("[WorkoutDOM] Workout form reset to default state.");
        console.groupEnd();
    },

    /**
     * Renders the trainer access requests bar on the Workout page or dashboard.
     *
     * @param {Array<object>} requests List of trainer requests returned by the API.
     */
    showTrainerRequests(requests) {
        const container = document.getElementById("trainerRequestsContainer");
        if (!container) {
            console.warn("[WorkoutDOM] showTrainerRequests: #trainerRequestsContainer not found.");
            return;
        }

        console.group("[WorkoutDOM] showTrainerRequests");
        console.debug("[WorkoutDOM] Incoming trainer requests:", requests);

        if (!requests || requests.length === 0) {
            console.info("[WorkoutDOM] No trainer requests to display.");
            container.innerHTML = `
                <p class="text-muted">No trainer requests.</p>
            `;
            console.groupEnd();
            return;
        }

        container.innerHTML = requests.map(r => {
            console.debug("[WorkoutDOM] Rendering trainer request:", r);
            return `
                <div class="alert alert-light d-flex justify-content-between align-items-center">
                    <div>
                        <strong>${r.trainerName}</strong> wants to train you.
                    </div>
                    <div>
                        <button class="btn btn-success btn-sm request-accept-btn"
                                data-request-id="${r.trainerClientRequestId}">
                            Accept
                        </button>
                        <button class="btn btn-danger btn-sm request-decline-btn"
                                data-request-id="${r.trainerClientRequestId}">
                            Decline
                        </button>
                    </div>
                </div>
            `;
        }).join('');

        console.info("[WorkoutDOM] Rendered", requests.length, "trainer request(s).");
        console.groupEnd();
    }

};

export { WorkoutDOM };
