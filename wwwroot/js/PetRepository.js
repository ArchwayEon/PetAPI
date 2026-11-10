// PetRepository: a small JavaScript class that wraps the Pet Web API.
// Every method returns a Promise, so callers use "await".
export class PetRepository {
    #baseAddress;

    constructor(baseAddress) {
        this.#baseAddress = baseAddress; // e.g. "/api/pets"
    }

    async #send(path = "", options = {}) {
        const response = await fetch(`${this.#baseAddress}${path}`, options);
        if (!response.ok) {
            const detail = await response.text();
            throw new Error(`HTTP ${response.status} ${response.statusText}: ${detail}`);
        }
        if (response.status === 204) return null;
        return response.json();
    }

    readAll() {
        return this.#send();
    }

    read(id) {
        return this.#send(`/${encodeURIComponent(id)}`);
    }

    create(formData) {
        return this.#send("", { method: "POST", body: formData });
    }

    update(id, formData) {
        return this.#send(`/${encodeURIComponent(id)}`, { method: "PUT", body: formData });
    }

    delete(id) {
        return this.#send(`/${encodeURIComponent(id)}`, { method: "DELETE" });
    }
}
