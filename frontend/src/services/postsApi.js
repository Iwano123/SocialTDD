import { authenticatedFetch } from '../utils/apiClient';

const API_BASE_URL = 'http://localhost:5000/api';

export const postsApi = {
  // Hämta tidslinje för en användare
  async getTimeline(userId) {
    const response = await authenticatedFetch(`${API_BASE_URL}/posts/timeline`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || 'Kunde inte hämta tidslinje');
    }

    return await response.json();
  },

  // Skapa ett nytt inlägg
  async createPost(recipientId, message) {
    const response = await authenticatedFetch(`${API_BASE_URL}/posts`, {
      method: 'POST',
      body: JSON.stringify({
        recipientId: recipientId,
        message: message,
      }),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || 'Kunde inte skapa inlägg');
    }

    return await response.json();
  },
};