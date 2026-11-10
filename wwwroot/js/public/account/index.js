function switchTab(tabName) {
    document.querySelectorAll('.tab-content').forEach(el => el.classList.add('hidden'));
    document.querySelectorAll('.tab-btn').forEach(el => {
        el.classList.remove('border-[#1e3a5f]', 'text-[#1e3a5f]');
        el.classList.add('border-transparent', 'text-slate-500');
    });

    document.getElementById(`tab-content-${tabName}`).classList.remove('hidden');
    const activeBtn = document.getElementById(`tab-btn-${tabName}`);
    activeBtn.classList.remove('border-transparent', 'text-slate-500');
    activeBtn.classList.add('border-[#1e3a5f]', 'text-[#1e3a5f]');
}

// Chế độ chỉnh sửa thông tin cá nhân
function toggleEditMode(isEditing) {
    if (isEditing) {
        document.getElementById('profile-view-mode').classList.add('hidden');
        document.getElementById('profile-edit-form').classList.remove('hidden');
    } else {
        document.getElementById('profile-edit-form').classList.add('hidden');
        document.getElementById('profile-view-mode').classList.remove('hidden');
    }
}

async function submitUpdateContact(e) {
    e.preventDefault();
    const phone = document.getElementById('input-phone').value.trim();
    const email = document.getElementById('input-email').value.trim();
    const address = document.getElementById('input-address').value.trim();

    if (!phone) {
        showToast("Số điện thoại không được để trống.");
        return;
    }

    // Kiểm tra định dạng số điện thoại Việt Nam (10 chữ số, bắt đầu bằng số 0)
    const phoneRegex = /^0\d{9}$/;
    if (!phoneRegex.test(phone)) {
        showToast("Số điện thoại không hợp lệ! Phải có đúng 10 chữ số và bắt đầu bằng số 0.");
        return;
    }

    if (!email) {
        showToast("Email không được để trống.");
        return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        showToast("Định dạng email không hợp lệ.");
        return;
    }

    if (!address) {
        showToast("Địa chỉ không được để trống.");
        return;
    }

    const updatedData = {
        phone: phone,
        email: email,
        address: address
    };

    try {
        const response = await apiClient.private.put(API.private.auth.updateContact, updatedData);

        if (response.ok) {
            document.getElementById('lbl-phone').innerText = updatedData.phone;
            document.getElementById('lbl-email').innerText = updatedData.email;
            document.getElementById('lbl-address').innerText = updatedData.address;

            toggleEditMode(false);

            const alertBox = document.getElementById('info-success-alert');
            if (alertBox) {
                alertBox.classList.remove('hidden');
                setTimeout(() => alertBox.classList.add('hidden'), 2500);
            }
        } else {
            showToast("Không thể cập nhật thông tin.");
        }
    } catch (err) {
        let errorMsg = "Có lỗi xảy ra khi cập nhật thông tin.";
        if (err && err.response && err.response.data && err.response.data.message) {
            errorMsg = err.response.data.message;
        } else if (err && err.message) {
            errorMsg = err.message;
        }
        showToast(errorMsg);
    }
}

let countdownInterval = null;

function openRenewalModal() {
    const modal = document.getElementById('renewal-modal');
    if (modal) {
        modal.classList.remove('hidden');
        // Reset về step 1
        document.getElementById('renewal-step-select').classList.remove('hidden');
        document.getElementById('renewal-step-transfer').classList.add('hidden');
    }
}

function closeRenewalModal() {
    const modal = document.getElementById('renewal-modal');
    if (modal) {
        modal.classList.add('hidden');
        if (countdownInterval) clearInterval(countdownInterval);
    }
}

function switchToBankTransfer() {
    document.getElementById('renewal-step-select').classList.add('hidden');
    document.getElementById('renewal-step-transfer').classList.remove('hidden');
    startRenewalCountdown(900); // 15 phút = 900 giây
}

function startRenewalCountdown(durationSeconds) {
    if (countdownInterval) clearInterval(countdownInterval);
    let timer = durationSeconds;
    const countdownEl = document.getElementById('renewal-countdown');
    const qrBox = document.getElementById('qr-code-box');
    const expiredBox = document.getElementById('qr-expired-box');

    // Reset UI trạng thái QR
    if (qrBox) qrBox.classList.remove('hidden');
    if (expiredBox) expiredBox.classList.add('hidden');
    if (countdownEl) {
        countdownEl.className = "font-mono text-sm font-bold tabular-nums px-2 py-0.5 rounded text-amber-700 bg-amber-50 border border-amber-200";
    }

    countdownInterval = setInterval(() => {
        const minutes = Math.floor(timer / 60);
        const seconds = timer % 60;

        if (countdownEl) {
            countdownEl.textContent = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
        }

        if (--timer < 0) {
            clearInterval(countdownInterval);
            if (countdownEl) {
                countdownEl.textContent = "00:00";
                countdownEl.className = "font-mono text-sm font-bold tabular-nums px-2 py-0.5 rounded text-rose-700 bg-rose-50 border border-rose-200";
            }
            if (qrBox) qrBox.classList.add('hidden');
            if (expiredBox) expiredBox.classList.remove('hidden');
        }
    }, 1000);
}

function copyToClipboard(text, labelId) {
    navigator.clipboard.writeText(text).then(() => {
        const label = document.getElementById(labelId);
        if (label) {
            label.classList.remove('hidden');
            setTimeout(() => label.classList.add('hidden'), 2000);
        }
    });
}

function copyElementText(elementId, labelId) {
    const text = document.getElementById(elementId)?.innerText || '';
    copyToClipboard(text, labelId);
}

async function handleOnlineRenewal() {
    try {
        const response = await apiClient.private.post('/api/account/renew-card', {});

        if (response.ok) {
            closeRenewalModal();
            showToast("Gia hạn thẻ trực tuyến thành công! Vui lòng tải lại trang.");
            setTimeout(() => location.reload(), 1500); // Reload để nhận hạn mới từ Server
        } else {
            showToast("Gia hạn thất bại.");
        }
    } catch (err) {
        showToast("Có lỗi xảy ra khi gia hạn thẻ.");
    }
}

function handleInPersonRenewal() {
    closeRenewalModal();
    const noticeBox = document.getElementById('in-person-notice-box');
    if (noticeBox) {
        noticeBox.classList.remove('hidden');
    }
}

// Lưu cài đặt thông báo gọi API thật
async function saveSettingsAPI() {
    const settings = {
        notifyDue: document.getElementById('setting-notify-due').checked,
        notifyReserved: document.getElementById('setting-notify-reserved').checked,
        notifyEvents: document.getElementById('setting-notify-events').checked
    };

    try {
        const response = await apiClient.private.post('/api/account/settings', settings);
        showToast("Đã lưu cấu hình thông báo thành công!");
    } catch (err) {
        showToast("Không thể lưu cấu hình thông báo.");
    }
}


async function submitUpdatePassword(event) {
    event.preventDefault();

    const currentPassword = document.getElementById('sec-current-pw').value;
    const newPassword = document.getElementById('sec-new-pw').value;
    const confirmPassword = document.getElementById('sec-confirm-pw').value;

    const successAlert = document.getElementById('security-success-alert');
    const passwordRegex = /^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&._-]).{8,}$/;

    if (!currentPassword) {
        showToast("Vui lòng nhập mật khẩu hiện tại.");
        return;
    }

    if (!passwordRegex.test(newPassword)) {
        showToast("Mật khẩu mới không đạt chuẩn bảo mật!\n- Phải có tối thiểu 8 ký tự.\n- Phải chứa ít nhất 1 chữ hoa, 1 chữ số và 1 ký tự đặc biệt (@$!%*?&._-).");
        return;
    }

    if (newPassword !== confirmPassword) {
        showToast("Mật khẩu mới và xác nhận mật khẩu không khớp!");
        return;
    }

    try {
        const response = await apiClient.private.post(API.private.auth.changePassword, {
            CurrentPassword: currentPassword,
            NewPassword: newPassword,
            ConfirmPassword: confirmPassword
        });


        if (response.ok) {
            if (successAlert) {
                successAlert.classList.remove('hidden');
                setTimeout(() => {
                    successAlert.classList.add('hidden');
                }, 3000);
            }
            event.target.reset();
        } else {
            showToast("Không thể đổi mật khẩu, vui lòng kiểm tra lại thông tin.");
        }
    } catch (error) {
        let msg = "Đã xảy ra lỗi kết nối đến máy chủ.";
        if (error && error.message) msg = error.message;
        showToast(msg);
    }
}

function togglePasswordVisibility(inputId, iconId) {
    const input = document.getElementById(inputId);
    const icon = document.getElementById(iconId);

    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.remove('fa-eye');
        icon.classList.add('fa-eye-slash');
    } else {
        input.type = 'password';
        icon.classList.remove('fa-eye-slash');
        icon.classList.add('fa-eye');
    }
}

function openLostCardModal() {
    const modal = document.getElementById('lost-card-modal');
    if (modal) {
        modal.classList.remove('hidden');
    }
}

function closeLostCardModal() {
    const modal = document.getElementById('lost-card-modal');
    if (modal) {
        modal.classList.add('hidden');
    }
}

async function executeReportLost() {
    closeLostCardModal();

    try {
        const response = await apiClient.private.post(API.private.auth.reportLost
        );

        if (response.ok) {
            showToast("Đã khóa thẻ thành công! Vui lòng liên hệ thủ thư nếu tìm lại được thẻ.");
            setTimeout(() => location.reload(), 1500); // Reload lại trang để cập nhật trạng thái mới nhất từ DB
        } else {
            showToast("Không thể thực hiện yêu cầu báo mất thẻ.");
        }
    } catch (err) {
        let errorMsg = "Có lỗi xảy ra khi báo mất thẻ.";
        if (err && err.response && err.response.data && err.response.data.message) {
            errorMsg = err.response.data.message;
        } else if (err && err.message) {
            errorMsg = err.message;
        }
        showToast(errorMsg);
    }
}