'use strict';

/**
 * Repository for trainer request operations.
 *
 * Expected API endpoints (relative to base address):
 *  - GET  {base}/mine     → returns trainer requests for the current user
 *  - POST {base}/respond  → accepts/declines a specific request
 */
export class TrainerRequestAJAXRepository {

    #base;

    /**
     * @param {string} baseAddress Base URL for the trainer request API (e.g. "/api/trainerrequest").
     */
    constructor(baseAddress) {
        this.#base = baseAddress;
        console.info("[TrainerRequestRepo] Initialized with base:", this.#base);
    }

    /**
     * Loads all trainer requests for the current logged-in user.
     * Used on the client dashboard to show pending trainer access requests.
     */
    async readMine() {
        const url = `${this.#base}/mine`;
        console.group("[TrainerRequestRepo] readMine");
        console.debug("[TrainerRequestRepo] GET", url);

        try {
            const r = await fetch(url);

            if (!r.ok) {
                console.error(
                    "[TrainerRequestRepo] Failed to load trainer requests.",
                    { status: r.status, statusText: r.statusText }
                );
                console.groupEnd();
                throw new Error("Error loading trainer requests.");
            }

            const data = await r.json();
            console.debug("[TrainerRequestRepo] Trainer requests loaded:", data);
            console.groupEnd();
            return data;

        } catch (err) {
            console.error("[TrainerRequestRepo] Network or parsing error in readMine.", err);
            console.groupEnd();
            throw err;
        }
    }

    /**
     * Sends a decision (accept/decline) for a trainer request.
     * Expects FormData with:
     *  - RequestId
     *  - Decision ("accept" | "decline")
     */
    async respond(formData) {
        const url = `${this.#base}/respond`;
        console.group("[TrainerRequestRepo] respond");
        console.debug("[TrainerRequestRepo] POST", url);

        // Optional: log the decision without dumping everything
        const decision = formData.get("Decision");
        const requestId = formData.get("RequestId");
        console.debug("[TrainerRequestRepo] Payload summary:", {
            RequestId: requestId,
            Decision: decision
        });

        try {
            const r = await fetch(url, {
                method: "POST",
                body: formData
            });

            if (!r.ok) {
                console.error(
                    "[TrainerRequestRepo] Failed to respond to trainer request.",
                    { status: r.status, statusText: r.statusText }
                );
                console.groupEnd();
                throw new Error("Error responding to request.");
            }

            const resultText = await r.text();
            console.debug("[TrainerRequestRepo] Response text:", resultText);
            console.groupEnd();
            return resultText;

        } catch (err) {
            console.error("[TrainerRequestRepo] Network or server error in respond.", err);
            console.groupEnd();
            throw err;
        }
    }
}
