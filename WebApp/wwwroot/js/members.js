/* ChatGPT */

document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.getElementById('memberSearch');
    const showButton = document.getElementById('btnShowMembers');
    const dropdown = document.getElementById('membersDropdown');

    // Check if elements exist before proceeding
    if (!searchInput || !showButton || !dropdown) {
        console.error('Required elements not found for member selection');
        return;
    }

    const noResults = dropdown.querySelector('.no-results');
    const memberItems = dropdown.querySelectorAll('.member-item');
    const selectedMembersContainer = document.querySelector('.selected-members');

    // Ensure we have the container for selected members
    if (!selectedMembersContainer) {
        console.error('Selected members container not found');
        return;
    }

    // Show/hide dropdown
    showButton.addEventListener('click', () => {
        dropdown.style.display = dropdown.style.display === 'block' ? 'none' : 'block';
        filterMembers();
    });

    // Filter members as you type
    searchInput.addEventListener('input', () => {
        filterMembers();
    });

    function filterMembers() {
        const term = searchInput.value.toLowerCase();
        let found = false;

        // Check if we have member items
        if (memberItems.length === 0) {
            if (noResults) noResults.style.display = 'block';
            return;
        }

        memberItems.forEach(item => {
            const name = item.dataset.name.toLowerCase();
            const id = item.dataset.id;
            const alreadySelected = selectedMembersContainer.querySelector(`[data-id="${id}"]`);

            if (name.includes(term) && !alreadySelected) {
                item.style.display = 'flex';
                found = true;
            } else {
                item.style.display = 'none';
            }
        });

        if (noResults) {
            noResults.style.display = found ? 'none' : 'block';
        }
    }

    // Add member when clicked
    memberItems.forEach(item => {
        item.addEventListener('click', () => {
            const id = item.dataset.id;
            const name = item.dataset.name;

            if (selectedMembersContainer.querySelector(`[data-id="${id}"]`)) return;

            const memberDiv = document.createElement('div');
            memberDiv.classList.add('selected-member');
            memberDiv.dataset.id = id;
            memberDiv.innerHTML = `
                <span>${name}</span>
                <button type="button" class="btn btn-remove-member">×</button>
                <input type="hidden" name="SelectedMemberIds" value="${id}" />
            `;

            // Insert before the search container
            const searchContainer = selectedMembersContainer.querySelector('.search-container');
            if (searchContainer) {
                selectedMembersContainer.insertBefore(memberDiv, searchContainer);
            } else {
                selectedMembersContainer.appendChild(memberDiv);
            }

            searchInput.value = '';
            filterMembers();
        });
    });

    // Remove member when × is clicked
    selectedMembersContainer.addEventListener('click', e => {
        if (e.target.classList.contains('btn-remove-member')) {
            e.target.closest('.selected-member').remove();
            filterMembers();
        }
    });

    // Hide dropdown when clicking outside
    document.addEventListener('click', e => {
        if (dropdown && !dropdown.contains(e.target) &&
            !searchInput.contains(e.target) &&
            !showButton.contains(e.target)) {
            dropdown.style.display = 'none';
        }
    });

    // Initial filter to hide already selected members
    filterMembers();
});
