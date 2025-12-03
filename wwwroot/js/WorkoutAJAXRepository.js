'use strict';

/**
 * AJAX repository for workout-related REST calls.
 *
 * This class wraps all fetch requests to the Workout Web API and is used
 * by the Workout page to perform asynchronous CRUD operations.
 */
export class WorkoutAJAXRepository {

    #base;

    /**
     * Creates a new instance of WorkoutAJAXRepository.
     * @param {string} baseAddress Base URL for the workout API (e.g. "/api/workout").
     */
    constructor(baseAddress) {
        this.#base = baseAddress;
        console.info("[WorkoutRepo] Initialized with base:", this.#base);
    }

    /**
     * Reads all workouts for the current user.
     *
     * Sends GET {base}/all and returns the list of workouts in JSON format.
     * Used to render the workout cards on the Workout page.
     *
     * @returns {Promise<Array>} A promise that resolves to an array of workout objects.
     * @throws {Error} If the request fails.
     */
    async readAll() {
        const url = `${this.#base}/all`;
        console.group("[WorkoutRepo] readAll");
        console.debug("[WorkoutRepo] GET", url);

        try {
            const r = await fetch(url);

            if (!r.ok) {
                console.error(
                    "[WorkoutRepo] Failed to load workouts.",
                    { status: r.status, statusText: r.statusText }
                );
                console.groupEnd();
                throw new Error("Error loading workouts.");
            }

            const data = await r.json();
            console.debug("[WorkoutRepo] Workouts loaded:", data);
            console.groupEnd();
            return data;

        } catch (err) {
            console.error("[WorkoutRepo] Network or parsing error in readAll.", err);
            console.groupEnd();
            throw err;
        }
    }

    /**
     * Reads a single workout by its ID.
     *
     * Sends GET {base}/one/{id}. Called when the user clicks "Edit" on a workout card
     * so the edit form can be pre-filled with the existing data.
     *
     * @param {number} id The workout ID to load.
     * @returns {Promise<object>} A promise that resolves to the workout object.
     * @throws {Error} If the request fails.
     */
    async read(id) {
        const url = `${this.#base}/one/${id}`;
        console.group("[WorkoutRepo] read");
        console.debug("[WorkoutRepo] GET", url, { id });

        try {
            const r = await fetch(url);

            if (!r.ok) {
                console.error(
                    "[WorkoutRepo] Failed to load workout.",
                    { status: r.status, statusText: r.statusText, id }
                );
                console.groupEnd();
                throw new Error("Error loading workout.");
            }

            const data = await r.json();
            console.debug("[WorkoutRepo] Workout loaded:", data);
            console.groupEnd();
            return data;

        } catch (err) {
            console.error("[WorkoutRepo] Network or parsing error in read.", err);
            console.groupEnd();
            throw err;
        }
    }

    /**
     * Creates a new workout using form data from the Workout page.
     *
     * Sends POST {base}/create with a FormData body. On success, the API returns
     * the created workout as JSON.
     *
     * @param {FormData} formData FormData containing workout fields.
     * @returns {Promise<object>} A promise that resolves to the created workout object.
     * @throws {Error} If the request fails.
     */
    async create(formData) {
        const url = `${this.#base}/create`;
        console.group("[WorkoutRepo] create");
        console.debug("[WorkoutRepo] POST", url);

        // Optional: log a small summary of the payload
        console.debug("[WorkoutRepo] Payload summary (create):", {
            WorkoutStyle: formData.get("WorkoutStyle"),
            DurationMinutes: formData.get("DurationMinutes"),
            TotalSets: formData.get("TotalSets"),
            TotalReps: formData.get("TotalReps")
        });

        try {
            const r = await fetch(url, {
                method: "POST",
                body: formData
            });

            if (!r.ok) {
                console.error(
                    "[WorkoutRepo] Failed to create workout.",
                    { status: r.status, statusText: r.statusText }
                );
                console.groupEnd();
                throw new Error("Error creating workout.");
            }

            const data = await r.json();
            console.debug("[WorkoutRepo] Workout created:", data);
            console.groupEnd();
            return data;

        } catch (err) {
            console.error("[WorkoutRepo] Network or server error in create.", err);
            console.groupEnd();
            throw err;
        }
    }

    /**
     * Updates an existing workout using form data from the Workout page.
     *
     * Sends PUT {base}/update with a FormData body. The API returns 204 No Content
     * on success, so this method returns the raw response text for debugging.
     *
     * @param {FormData} formData FormData containing updated workout fields, including WorkoutId.
     * @returns {Promise<string>} A promise that resolves to the response text.
     * @throws {Error} If the request fails.
     */
    async update(formData) {
        const url = `${this.#base}/update`;
        console.group("[WorkoutRepo] update");
        console.debug("[WorkoutRepo] PUT", url);

        console.debug("[WorkoutRepo] Payload summary (update):", {
            WorkoutId: formData.get("WorkoutId"),
            WorkoutStyle: formData.get("WorkoutStyle"),
            DurationMinutes: formData.get("DurationMinutes"),
            TotalSets: formData.get("TotalSets"),
            TotalReps: formData.get("TotalReps")
        });

        try {
            const r = await fetch(url, {
                method: "PUT",
                body: formData
            });

            if (!r.ok) {
                console.error(
                    "[WorkoutRepo] Failed to update workout.",
                    { status: r.status, statusText: r.statusText }
                );
                console.groupEnd();
                throw new Error("Error updating workout.");
            }

            const text = await r.text();
            console.debug("[WorkoutRepo] Update response text:", text);
            console.groupEnd();
            return text;

        } catch (err) {
            console.error("[WorkoutRepo] Network or server error in update.", err);
            console.groupEnd();
            throw err;
        }
    }

    /**
     * Deletes an existing workout for the current user.
     *
     * Sends DELETE {base}/delete/{id}. On success, the server responds with 204 No Content.
     * The return value is the raw response text.
     *
     * @param {number} id The workout ID to delete.
     * @returns {Promise<string>} A promise that resolves to the response text.
     * @throws {Error} If the request fails.
     */
    async delete(id) {
        const url = `${this.#base}/delete/${id}`;
        console.group("[WorkoutRepo] delete");
        console.debug("[WorkoutRepo] DELETE", url, { id });

        try {
            const r = await fetch(url, {
                method: "DELETE"
            });

            if (!r.ok) {
                console.error(
                    "[WorkoutRepo] Failed to delete workout.",
                    { status: r.status, statusText: r.statusText, id }
                );
                console.groupEnd();
                throw new Error("Error deleting workout.");
            }

            const text = await r.text();
            console.debug("[WorkoutRepo] Delete response text:", text);
            console.groupEnd();
            return text;

        } catch (err) {
            console.error("[WorkoutRepo] Network or server error in delete.", err);
            console.groupEnd();
            throw err;
        }
    }
}
