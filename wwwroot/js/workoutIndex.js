'use strict';

import { WorkoutDOM } from "/js/WorkoutDOM.js";
import { WorkoutAJAXRepository } from "/js/WorkoutAJAXRepository.js";
import { TrainerRequestAJAXRepository } from "/js/TrainerRequestAJAXRepository.js";

let workoutRepo;
let requestRepo;

// =====================================
// MAIN ENTRY POINT
// =====================================

/**
 * Initializes the Workout page:
 *  - Configures AJAX repositories
 *  - Loads trainer requests
 *  - Wires up DOM event handlers
 *  - Loads current workouts for the logged-in user
 */
main();

async function main() {
    console.group("[WorkoutIndex] main()");

    try {
        console.info("[WorkoutIndex] Initializing repositories…");
        workoutRepo = new WorkoutAJAXRepository('/api/workout');
        requestRepo = new TrainerRequestAJAXRepository('/api/trainerrequest');

        console.info("[WorkoutIndex] Loading trainer requests…");
        await loadTrainerRequests();

        console.info("[WorkoutIndex] Setting up DOM event handlers…");
        await setupEvents();

        console.info("[WorkoutIndex] Loading workouts for current user…");
        WorkoutDOM.showLoading();

        const workouts = await workoutRepo.readAll();
        console.debug("[WorkoutIndex] Workouts loaded:", workouts);

        WorkoutDOM.showWorkoutCards(workouts);
        console.info("[WorkoutIndex] Workout cards rendered successfully.");
    }
    catch (err) {
        console.error("[WorkoutIndex] Failed to initialize workout page:", err);
        WorkoutDOM.showAlert("Error loading workouts.", "danger");
    }
    finally {
        WorkoutDOM.hideLoading();
        console.groupEnd();
    }
}

// ================================
// TRAINER REQUESTS
// ================================

/**
 * Loads trainer access requests for the current user and
 * passes them to WorkoutDOM for rendering.
 */
async function loadTrainerRequests() {
    console.group("[WorkoutIndex] loadTrainerRequests");

    try {
        const requests = await requestRepo.readMine();
        console.debug("[WorkoutIndex] Trainer requests loaded:", requests);

        WorkoutDOM.showTrainerRequests(requests);
        console.info("[WorkoutIndex] Trainer requests rendered.");
    } catch (err) {
        console.warn("[WorkoutIndex] Could not load trainer requests. Rendering empty state.", err);
        WorkoutDOM.showTrainerRequests([]);
    } finally {
        console.groupEnd();
    }
}

// ================================
// EVENT HANDLERS
// ================================

/**
 * Wires up all DOM event handlers used on the Workout page:
 *  - Form submit (create / update)
 *  - Cancel edit
 *  - Global click (edit, delete, accept / decline trainer requests)
 */
async function setupEvents() {
    console.group("[WorkoutIndex] setupEvents");

    const workoutForm = document.getElementById('workoutForm');
    const cancelBtn = document.getElementById('cancelBtn');

    if (!workoutForm) {
        console.error("[WorkoutIndex] setupEvents: #workoutForm not found in DOM.");
        console.groupEnd();
        return;
    }

    if (!cancelBtn) {
        console.error("[WorkoutIndex] setupEvents: #cancelBtn not found in DOM.");
        console.groupEnd();
        return;
    }

    // --------------------------------
    // SUBMIT FORM (CREATE / UPDATE)
    // --------------------------------
    workoutForm.addEventListener("submit", async (e) => {
        e.preventDefault();

        const btn = document.getElementById("submitBtn");
        const editing = btn && btn.textContent.includes("Update");

        console.group("[WorkoutIndex] Form submit");
        console.info("[WorkoutIndex] Mode:", editing ? "Update existing workout" : "Create new workout");

        try {
            const form = new FormData(workoutForm);
            console.debug("[WorkoutIndex] FormData keys:", Array.from(form.keys()));

            if (editing) {
                console.info("[WorkoutIndex] Sending update request to API…");
                await workoutRepo.update(form);
                WorkoutDOM.showAlert("Workout updated!", "info");
            } else {
                console.info("[WorkoutIndex] Sending create request to API…");
                await workoutRepo.create(form);
                WorkoutDOM.showAlert("Workout added!", "success");
            }

            console.info("[WorkoutIndex] Reloading workout list after save…");
            const workouts = await workoutRepo.readAll();
            WorkoutDOM.showWorkoutCards(workouts);
            WorkoutDOM.resetForm();

            console.info("[WorkoutIndex] Form handled successfully.");
        } catch (err) {
            console.error("[WorkoutIndex] Error saving workout:", err);
            WorkoutDOM.showAlert("Error saving workout.", "danger");
        } finally {
            console.groupEnd();
        }
    });

    // --------------------------------
    // CANCEL BUTTON (RESET FORM)
    // --------------------------------
    cancelBtn.addEventListener("click", () => {
        console.info("[WorkoutIndex] Cancel clicked. Resetting form.");
        WorkoutDOM.resetForm();
    });

    // --------------------------------
    // GLOBAL CLICK HANDLER
    //  - Edit workout
    //  - Delete workout
    //  - Accept / Decline trainer request
    // --------------------------------
    document.addEventListener("click", async (e) => {

        // DELETE
        const del = e.target.closest(".delete-btn");
        if (del) {
            const id = del.dataset.workoutId;
            console.group("[WorkoutIndex] Delete workout");
            console.info("[WorkoutIndex] Deleting workout with ID:", id);

            try {
                await workoutRepo.delete(id);
                WorkoutDOM.showAlert("Workout deleted.", "warning");

                console.info("[WorkoutIndex] Reloading workouts after delete…");
                const workouts = await workoutRepo.readAll();
                WorkoutDOM.showWorkoutCards(workouts);
            } catch (err) {
                console.error("[WorkoutIndex] Error deleting workout:", err);
                WorkoutDOM.showAlert("Error deleting workout.", "danger");
            } finally {
                console.groupEnd();
            }
            return;
        }

        // EDIT
        const edit = e.target.closest(".edit-btn");
        if (edit) {
            const id = edit.dataset.workoutId;
            console.group("[WorkoutIndex] Edit workout");
            console.info("[WorkoutIndex] Loading workout for edit. ID:", id);

            try {
                const workout = await workoutRepo.read(id);
                console.debug("[WorkoutIndex] Workout loaded for edit:", workout);

                WorkoutDOM.populateFormForEdit(workout);
            } catch (err) {
                console.error("[WorkoutIndex] Error loading workout for edit:", err);
                WorkoutDOM.showAlert("Error loading workout for edit.", "danger");
            } finally {
                console.groupEnd();
            }
            return;
        }

        // ACCEPT TRAINER REQUEST
        const accept = e.target.closest(".request-accept-btn");
        if (accept) {
            const requestId = accept.dataset.requestId;
            console.group("[WorkoutIndex] Accept trainer request");
            console.info("[WorkoutIndex] Accepting request ID:", requestId);

            try {
                const fd = new FormData();
                fd.append("RequestId", requestId);
                fd.append("Decision", "accept");

                await requestRepo.respond(fd);
                console.info("[WorkoutIndex] Request accepted. Reloading requests…");
                await loadTrainerRequests();
                WorkoutDOM.showAlert("Trainer request accepted!", "success");
            } catch (err) {
                console.error("[WorkoutIndex] Error accepting trainer request:", err);
                WorkoutDOM.showAlert("Error accepting trainer request.", "danger");
            } finally {
                console.groupEnd();
            }
            return;
        }

        // DECLINE TRAINER REQUEST
        const decline = e.target.closest(".request-decline-btn");
        if (decline) {
            const requestId = decline.dataset.requestId;
            console.group("[WorkoutIndex] Decline trainer request");
            console.info("[WorkoutIndex] Declining request ID:", requestId);

            try {
                const fd = new FormData();
                fd.append("RequestId", requestId);
                fd.append("Decision", "decline");

                await requestRepo.respond(fd);
                console.info("[WorkoutIndex] Request declined. Reloading requests…");
                await loadTrainerRequests();
                WorkoutDOM.showAlert("Request declined.", "warning");
            } catch (err) {
                console.error("[WorkoutIndex] Error declining trainer request:", err);
                WorkoutDOM.showAlert("Error declining trainer request.", "danger");
            } finally {
                console.groupEnd();
            }
            return;
        }
    });

    console.info("[WorkoutIndex] Event handlers successfully wired.");
    console.groupEnd();
}
