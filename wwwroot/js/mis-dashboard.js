/* ─────────────────────────────────────────
   BookHive — MIS Dashboard Scripts
   ───────────────────────────────────────── */

// ── Search Filter ──────────────────────────
function filterTable() {
    const input = document.getElementById('searchInput').value.toLowerCase();
    const rows = document.querySelectorAll('#usersTable tbody tr');

    rows.forEach(function (row) {
        const text = row.textContent.toLowerCase();
        row.style.display = text.includes(input) ? '' : 'none';
    });
}

// ── Role Filter ────────────────────────────
function filterRole(btn, role) {
    // Remove active from all buttons
    document.querySelectorAll('.filter-btn').forEach(function (b) {
        b.classList.remove('active');
    });

    // Set clicked button as active
    btn.classList.add('active');

    // Filter rows by role
    const rows = document.querySelectorAll('#usersTable tbody tr');
    rows.forEach(function (row) {
        if (role === 'All' || row.dataset.role === role) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
}