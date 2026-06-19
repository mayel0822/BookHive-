/* ─────────────────────────────────────────
   BookHive — MIS User Info Scripts
   ───────────────────────────────────────── */

function openModal(modalId) {
    document.getElementById(modalId).classList.remove('hidden');
    if (modalId === 'modalResetPass') {
        generatePassword();
    }
}

function closeModal(modalId) {
    document.getElementById(modalId).classList.add('hidden');
}

function showToast(message) {
    const toast = document.getElementById('toast');
    if (!toast) return;
    toast.textContent = message;
    toast.classList.remove('hidden');
    setTimeout(function () {
        toast.classList.add('hidden');
    }, 3000);
}

function generatePassword() {
    const letters = 'abcdefghijklmnopqrstuvwxyz';
    const numbers = '0123456789';
    const length = 10;
    let password = '';

    for (let i = 0; i < length; i++) {
        const useNumber = Math.random() < 0.4;
        password += useNumber
            ? numbers.charAt(Math.floor(Math.random() * numbers.length))
            : letters.charAt(Math.floor(Math.random() * letters.length));
    }

    document.getElementById('newPass').value = password;
}

function resetAuthenticator() {
    showToast('Authenticator reset. User must re-enroll on next login.');
}

window.addEventListener('click', function (e) {
    document.querySelectorAll('#modalResetPass, #modalArchive').forEach(function (modal) {
        if (e.target === modal) {
            modal.classList.add('hidden');
        }
    });
});