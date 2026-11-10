import { PetRepository } from "./PetRepository.js";

const repo = new PetRepository("/api/pets");

const tbody = document.querySelector("#petTable tbody");
const createForm = document.querySelector("#createPetForm");
const editForm = document.querySelector("#editPetForm");

// Read all: GET /api/pets
// (Module scripts run after the DOM is parsed, so the page is ready here.)
loadPets();

async function loadPets() {
    try {
        const pets = await repo.readAll();
        tbody.innerHTML = "";
        pets.forEach(pet => addPetRow(pet));
    } catch (err) {
        showError(err.message);
    }
}

// Read one: GET /api/pets/{id}
// One listener on the table handles buttons in every row (event delegation).
document.querySelector("#petTable").addEventListener("click", async (e) => {
    const button = e.target.closest(".details-btn");
    if (!button) return;

    try {
        const pet = await repo.read(button.dataset.id);
        document.querySelector("#detailId").textContent = pet.id;
        document.querySelector("#detailName").textContent = pet.name;
        document.querySelector("#detailWeight").textContent = pet.weight;

        // Fill the edit form so the pet can be updated.
        editForm.elements["Id"].value = pet.id;
        editForm.elements["Name"].value = pet.name;
        editForm.elements["Weight"].value = pet.weight;
        document.querySelector("#detailsCard").classList.remove("d-none");
    } catch (err) {
        showError(err.message);
    }
});

// Create: POST /api/pets
createForm.addEventListener("submit", async (e) => {
    e.preventDefault();
    try {
        const pet = await repo.create(new FormData(createForm));
        addPetRow(pet);
        createForm.reset();
    } catch (err) {
        showError(err.message);
    }
});

// Update: PUT /api/pets/{id}
editForm.addEventListener("submit", async (e) => {
    e.preventDefault();
    const formData = new FormData(editForm);
    const id = formData.get("Id");

    try {
        await repo.update(id, formData); // server returns 204 No Content, so this resolves to null
        const values = Object.fromEntries(formData);
        updatePetRow(id, { name: values.Name, weight: values.Weight });
        document.querySelector("#detailName").textContent = values.Name;
        document.querySelector("#detailWeight").textContent = values.Weight;
    } catch (err) {
        showError(err.message);
    }
});

// Delete: DELETE /api/pets/{id}
document.querySelector("#petTable").addEventListener("click", async (e) => {
    const button = e.target.closest(".delete-btn");
    if (!button) return;
    if (!confirm("Delete this pet?")) return;

    try {
        await repo.delete(button.dataset.id);
        button.closest("tr").remove();
        if (editForm.elements["Id"].value === button.dataset.id) {
            document.querySelector("#detailsCard").classList.add("d-none");
        }
    } catch (err) {
        showError(err.message);
    }
});

// ---------- Helpers ----------

function addPetRow(pet) {
    const row = document.createElement("tr");
    row.dataset.id = pet.id;
    row.innerHTML = `
        <td>${pet.id}</td>
        <td class="pet-name">${escapeHtml(pet.name)}</td>
        <td class="pet-weight">${escapeHtml(String(pet.weight))}</td>
        <td>
            <button class="btn btn-sm btn-info details-btn" data-id="${pet.id}">Details</button>
            <button class="btn btn-sm btn-danger delete-btn" data-id="${pet.id}">Delete</button>
        </td>`;
    tbody.appendChild(row);
}

function updatePetRow(id, pet) {
    const row = tbody.querySelector(`tr[data-id="${CSS.escape(String(id))}"]`);
    if (!row) return;
    row.querySelector(".pet-name").textContent = pet.name;
    row.querySelector(".pet-weight").textContent = pet.weight;
}

// Escaping user data before using innerHTML prevents cross-site scripting (XSS).
function escapeHtml(text) {
    const div = document.createElement("div");
    div.textContent = text;
    return div.innerHTML;
}

function showError(message) {
    const alert = document.querySelector("#errorAlert");
    alert.textContent = message;
    alert.classList.remove("d-none");
}
