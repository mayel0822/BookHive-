/* ─────────────────────────────────────────
   BookHive — MIS Registration Page Scripts
   ───────────────────────────────────────── */

// ── Switch Role Tab ────────────────────────
function switchTab(role) {
    // Hide all field groups
    document.getElementById('fields-Student').classList.add('hidden');
    document.getElementById('fields-Professor').classList.add('hidden');
    document.getElementById('fields-Librarian').classList.add('hidden');

    // Reset all tabs to inactive style
    document.querySelectorAll('.role-tab').forEach(function (tab) {
        tab.classList.remove('bg-[#1B2B5E]', 'text-white', 'border-[#1B2B5E]');
        tab.classList.add('bg-transparent', 'text-slate-500', 'border-slate-200');
    });

    // Show selected fields
    document.getElementById('fields-' + role).classList.remove('hidden');

    // Set active tab style
    const activeTab = document.getElementById('tab-' + role);
    activeTab.classList.remove('bg-transparent', 'text-slate-500', 'border-slate-200');
    activeTab.classList.add('bg-[#1B2B5E]', 'text-white', 'border-[#1B2B5E]');

    // Update hidden role field
    document.getElementById('roleField').value = role;

    // Update form title
    const titles = {
        Student: 'Student Registration',
        Professor: 'Professor Registration',
        Librarian: 'Librarian Registration'
    };
    document.getElementById('formTitle').textContent = titles[role];
}

// ── Dismiss Success Box ────────────────────
function dismissSuccess() {
    const box = document.getElementById('successBox');
    if (box) box.style.display = 'none';
}

// ── Form Validation ────────────────────────
const regForm = document.getElementById('regForm');

if (regForm) {
    regForm.addEventListener('submit', function (e) {
        const role = document.getElementById('roleField').value;
        let isValid = true;

        // Clear previous errors
        document.querySelectorAll('.form-input').forEach(function (input) {
            input.classList.remove('border-red-600');
        });

        // Get active fields container
        const container = document.getElementById('fields-' + role);
        const inputs = container.querySelectorAll('input, select');

        inputs.forEach(function (input) {
            // Skip optional RFID field
            if (input.name === 'rfid') return;

            if (!input.value.trim()) {
                input.classList.add('border-red-600');
                isValid = false;
            }

            // Phone validation
            if (input.name === 'phone' && input.value.length < 11) {
                input.classList.add('border-red-600');
                isValid = false;
            }

            // Email validation
            if (input.type === 'email' && !input.value.includes('@')) {
                input.classList.add('border-red-600');
                isValid = false;
            }
        });

        if (!isValid) {
            e.preventDefault();
            alert('Please fill in all required fields correctly.');
        }
    });
}