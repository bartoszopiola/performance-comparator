/**
 * compare.js — fund chip picker for the Compare form (vanilla JS).
 */
(function () {
    'use strict';

    const fundSelect = document.getElementById('fundSelect');
    const addBtn = document.getElementById('addFundBtn');
    const chipsBox = document.getElementById('fundChips');
    const hiddenBox = document.getElementById('fundHiddenInputs');
    const MAX_FUNDS = 4;

    if (!fundSelect || !addBtn || !chipsBox || !hiddenBox) return;

    function selectedIds() {
        return Array.from(hiddenBox.querySelectorAll('input[name="FundIds"]')).map(i => i.value);
    }

    function addFund() {
        const id = fundSelect.value;
        const name = fundSelect.options[fundSelect.selectedIndex]?.text;
        if (!id) return;

        const ids = selectedIds();
        if (ids.includes(id)) { alert('This fund is already added.'); return; }
        if (ids.length >= MAX_FUNDS) { alert('You can compare at most ' + MAX_FUNDS + ' funds.'); return; }

        const chip = document.createElement('span');
        chip.className = 'badge bg-primary me-2 mb-2 d-inline-flex align-items-center gap-1';
        chip.dataset.id = id;
        chip.textContent = name;

        const removeBtn = document.createElement('button');
        removeBtn.type = 'button';
        removeBtn.className = 'btn-close btn-close-white';
        removeBtn.style.fontSize = '0.65em';
        removeBtn.setAttribute('aria-label', 'Remove');
        removeBtn.addEventListener('click', function () {
            chip.remove();
            hiddenBox.querySelector('input[value="' + id + '"]')?.remove();
            updateState();
        });

        chip.appendChild(removeBtn);
        chipsBox.appendChild(chip);

        const hidden = document.createElement('input');
        hidden.type = 'hidden';
        hidden.name = 'FundIds';
        hidden.value = id;
        hiddenBox.appendChild(hidden);

        updateState();
    }

    function updateState() {
        const count = selectedIds().length;
        addBtn.disabled = count >= MAX_FUNDS;
        document.getElementById('fundCount').textContent = count + ' selected';
    }

    addBtn.addEventListener('click', addFund);
    fundSelect.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') { e.preventDefault(); addFund(); }
    });

    const form = document.getElementById('compareForm');
    if (form) {
        form.addEventListener('submit', function (e) {
            if (selectedIds().length === 0) {
                e.preventDefault();
                alert('Add at least one fund to compare.');
                return;
            }
            const benchId = document.getElementById('benchmarkSelect')?.value;
            if (!benchId || benchId === '0') {
                e.preventDefault();
                alert('Select a benchmark fund.');
            }
        });
    }
})();