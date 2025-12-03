'use strict';

/**
 * Builds dynamic set inputs for the assigned workout entry page.
 *
 * Expected DOM:
 *  - <input type="hidden" id="ExerciseData" value="[...]">
 *  - <div id="exerciseContainer"></div>
 *
 * This script:
 *  - Reads ExerciseData JSON
 *  - Safely parses it
 *  - Renders inputs for reps/weight per set
 *  - Logs each step to the browser console for debugging
 */
document.addEventListener("DOMContentLoaded", () => {
    console.group("[assignedEntry] Initialization");

    // Locate required DOM elements
    const rawField = document.getElementById("ExerciseData");
    const container = document.getElementById("exerciseContainer");

    if (!rawField) {
        console.warn("[assignedEntry] #ExerciseData not found. No dynamic sets will be rendered.");
        console.groupEnd();
        return;
    }

    if (!container) {
        console.warn("[assignedEntry] #exerciseContainer not found. Cannot render exercise UI.");
        console.groupEnd();
        return;
    }

    const raw = rawField.value ?? "";
    console.log("[assignedEntry] Raw ExerciseData value:", raw);

    if (!raw.trim()) {
        console.warn("[assignedEntry] ExerciseData is empty. Skipping UI build.");
        console.groupEnd();
        return;
    }

    let exercises;
    try {
        exercises = JSON.parse(raw);
        console.log("[assignedEntry] Parsed exercises:", exercises);
    } catch (err) {
        console.error("[assignedEntry] Failed to parse ExerciseData JSON.", err);
        console.groupEnd();
        return;
    }

    if (!Array.isArray(exercises) || exercises.length === 0) {
        console.warn("[assignedEntry] Parsed ExerciseData is not a non-empty array. Nothing to render.");
        console.groupEnd();
        return;
    }

    // Clear any previous content
    container.innerHTML = "";
    console.info(`[assignedEntry] Building UI for ${exercises.length} exercise(s).`);

    // Render each exercise and its sets
    exercises.forEach((ex, index) => {
        const safeName = ex.ExerciseName || `Exercise ${index + 1}`;
        const setCount = Number(ex.Sets) || 0;

        console.group(`[assignedEntry] Exercise ${index + 1}: ${safeName}`);
        console.log("[assignedEntry] Exercise details:", ex);
        console.log("[assignedEntry] Set count:", setCount);

        if (setCount <= 0) {
            console.warn("[assignedEntry] Exercise has no sets configured, skipping.", ex);
            console.groupEnd();
            return;
        }

        const block = document.createElement("div");
        block.className = "mb-4 p-3 bg-white border rounded";

        let html = `<h5 class="fw-bold mb-3">${safeName}</h5>`;

        for (let i = 1; i <= setCount; i++) {
            const repsFieldName = `SetReps_${safeName}_${i}`;
            const weightFieldName = `SetWeight_${safeName}_${i}`;

            console.log("[assignedEntry] Adding inputs for set:", {
                setNumber: i,
                repsFieldName,
                weightFieldName
            });

            html += `
              <div class="row mb-3">

                  <div class="col-md-6">
                      <label>Set ${i} Reps</label>
                      <input type="number"
                             class="form-control"
                             name="${repsFieldName}"
                             min="0"
                             required
                             oninput="this.value = this.value.replace(/[^0-9]/g,'');">
                  </div>

                  <div class="col-md-6">
                      <label>Set ${i} Weight</label>
                      <input type="number"
                             class="form-control"
                             name="${weightFieldName}"
                             min="0"
                             required
                             oninput="this.value = this.value.replace(/[^0-9]/g,'');">
                  </div>

              </div>
            `;
        }

        block.innerHTML = html;
        container.appendChild(block);
        console.groupEnd();
    });

    console.info("[assignedEntry] Finished building exercise UI.");
    console.groupEnd();
});
