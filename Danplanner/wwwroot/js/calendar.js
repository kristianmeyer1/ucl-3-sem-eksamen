// Range picker: select from-to, compute days, and persist to sessionStorage for later pages.

document.addEventListener("DOMContentLoaded", function () {
    const locale = (flatpickr && flatpickr.l10ns && flatpickr.l10ns.da) ? flatpickr.l10ns.da : "default";

    const startEl = document.getElementById("start");
    const endEl = document.getElementById("end");
    const daysEl = document.getElementById("days");
    const clearBtn = document.getElementById("clearSelection");

    function toISODate(d) {
        // Return yyyy-mm-dd
        const yyyy = d.getFullYear();
        const mm = String(d.getMonth() + 1).padStart(2, "0");
        const dd = String(d.getDate()).padStart(2, "0");
        return `${yyyy}-${mm}-${dd}`;
    }

    function computeDaysInclusive(startDate, endDate) {
        // compute inclusive day count
        const utcStart = Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate());
        const utcEnd = Date.UTC(endDate.getFullYear(), endDate.getMonth(), endDate.getDate());
        const msPerDay = 24 * 60 * 60 * 1000;
        return Math.floor((utcEnd - utcStart) / msPerDay) + 1;
    }

    function saveSelection(startIso, endIso, days) {
        sessionStorage.setItem("bookingStart", startIso || "");
        sessionStorage.setItem("bookingEnd", endIso || "");
        sessionStorage.setItem("bookingDays", String(days || 0));
    }

    function updateUI(startIso, endIso, days) {
        arrivalLabel.textContent = startIso ? (new Date(startIso)).toLocaleDateString("da-DK", { day: "numeric", month: "short" }) : "ingen";
        departureLabel.textContent = endIso ? (new Date(endIso)).toLocaleDateString("da-DK", { day: "numeric", month: "short" }) : "ingen";
        daysEl.textContent = String(days || 0);
    }

    const fp = flatpickr("#cal1", {
        locale: locale,
        inline: true,
        mode: "range",
        minDate: "today",
        allowInput: false,
        onChange: function (selectedDates, dateStr, instance) {
            if (!selectedDates || selectedDates.length === 0) {
                // cleared
                updateUI("", "", 0);
                saveSelection("", "", 0);
                return;
            }

            let start = selectedDates[0];
            let end = selectedDates.length > 1 ? selectedDates[1] : selectedDates[0];

            const startIso = toISODate(start);
            const endIso = toISODate(end);
            const days = computeDaysInclusive(start, end);

            updateUI(startIso, endIso, days);
            saveSelection(startIso, endIso, days);
        },
        onReady: function (selectedDates, dateStr, instance) {
            // restore previous selection from sessionStorage if present
            const storedStart = sessionStorage.getItem("bookingStart");
            const storedEnd = sessionStorage.getItem("bookingEnd");
            const storedDays = sessionStorage.getItem("bookingDays");

            if (storedStart && storedEnd) {
                // set the calendar selection (flatpickr accepts array of date strings)
                try {
                    instance.setDate([storedStart, storedEnd], false);
                    updateUI(storedStart, storedEnd, Number(storedDays || 0));
                } catch (e) {
                    // ignore malformed stored values
                    updateUI("", "", 0);
                    saveSelection("", "", 0);
                }
            } else {
                // initialize UI from instance if there is already a date
                if (selectedDates && selectedDates.length > 0) {
                    const s = selectedDates[0];
                    const e = selectedDates.length > 1 ? selectedDates[1] : s;
                    const sIso = toISODate(s);
                    const eIso = toISODate(e);
                    const d = computeDaysInclusive(s, e);
                    updateUI(sIso, eIso, d);
                    saveSelection(sIso, eIso, d);
                } else {
                    updateUI("", "", 0);
                }
            }
        }
    });

    if (clearBtn) {
        clearBtn.addEventListener("click", function () {
            fp.clear();
            updateUI("", "", 0);
            saveSelection("", "", 0);
        });
    }
});