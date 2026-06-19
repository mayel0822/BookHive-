/* ─────────────────────────────────────────
   BookHive — MIS Login Page Scripts
   ───────────────────────────────────────── */

// ── Toggle Password Visibility ─────────────
const togglePassword = document.getElementById('togglePassword');
const passwordInput = document.getElementById('password');

if (togglePassword && passwordInput) {
    togglePassword.addEventListener('click', function () {
        const isPassword = passwordInput.type === 'password';
        passwordInput.type = isPassword ? 'text' : 'password';
        togglePassword.textContent = isPassword ? '🙈' : '👁️';
    });
}

// ── Password Max Length ────────────────────
if (passwordInput) {
    passwordInput.setAttribute('maxlength', '20');
}

// ── Form Validation ────────────────────────
const loginForm = document.getElementById('loginForm');
const emailInput = document.getElementById('email');
const emailError = document.getElementById('emailError');
const passwordError = document.getElementById('passwordError');
const loginBtn = document.getElementById('loginBtn');
const btnText = document.getElementById('btnText');
const btnLoader = document.getElementById('btnLoader');

if (loginForm) {
    loginForm.addEventListener('submit', function (e) {
        let isValid = true;

        // Clear previous errors
        emailError.textContent = '';
        passwordError.textContent = '';
        emailInput.classList.remove('border-red-600');
        passwordInput.classList.remove('border-red-600');

        // Validate email
        const email = emailInput.value.trim();
        if (!email) {
            emailError.textContent = 'Email address is required.';
            emailInput.classList.add('border-red-600');
            isValid = false;
        } else if (!email.includes('@')) {
            emailError.textContent = 'Please enter a valid email address.';
            emailInput.classList.add('border-red-600');
            isValid = false;
        }

        // Validate password
        const password = passwordInput.value.trim();
        if (!password) {
            passwordError.textContent = 'Password is required.';
            passwordInput.classList.add('border-red-600');
            isValid = false;
        } else if (password.length < 6) {
            passwordError.textContent = 'Password must be at least 6 characters.';
            passwordInput.classList.add('border-red-600');
            isValid = false;
        } else if (password.length > 20) {
            passwordError.textContent = 'Password cannot exceed 20 characters.';
            passwordInput.classList.add('border-red-600');
            isValid = false;
        }

        // If invalid stop submission
        if (!isValid) {
            e.preventDefault();
            return;
        }

        // Show loading state
        btnText.classList.add('hidden');
        btnLoader.classList.remove('hidden');
        loginBtn.disabled = true;
    });
}

// ── Dismiss Error Box ──────────────────────
function dismissError() {
    const errorBox = document.getElementById('errorBox');
    if (errorBox) {
        errorBox.style.display = 'none';
    }
}

// ── Auto-focus Email Field ─────────────────
window.addEventListener('load', function () {
    if (emailInput) {
        emailInput.focus();
    }
});