'use strict';

console.log("=== DEBUG START ===");

// Check what the session username is showing (it will show NULL if server is not passing it)
const welcomeHeader = document.querySelector("h2");
console.log("Welcome header:", welcomeHeader?.innerText);

// Check if the dashboard DOM even exists
const dashboardSection = document.querySelector("section.container");
console.log("Dashboard section exists:", dashboardSection !== null);

// Check cookies (session ID)
console.log("Cookies:", document.cookie);

// Tell you visually on the page
if (!dashboardSection) {
    alert("DEBUG: You are NOT inside the dashboard. This means the server is redirecting you away.");
}
