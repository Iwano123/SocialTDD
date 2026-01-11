import { authenticatedFetch } from '../utils/apiClient';

const API_BASE_URL = 'http://localhost:5000/api';

export const wallApi = {
  // Hämta vägg (posts från följda användare)
  async getWall() {
    const response = await authenticatedFetch(`${API_BASE_URL}/wall`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || error.message || 'Kunde inte hämta vägg');
    }

    return await response.json();
  },
};