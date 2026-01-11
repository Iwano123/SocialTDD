const API_BASE_URL = 'http://localhost:5000/api';

export const postsApi = {
  // Hämta tidslinje för en användare
  async getTimeline(userId) {
    const response = await fetch(`${API_BASE_URL}/posts/timeline/${userId}`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || 'Kunde inte hämta tidslinje');
    }

    return await response.json();
  },

  // Skapa ett nytt inlägg
  async createPost(senderId, recipientId, message) {
    const response = await fetch(`${API_BASE_URL}/posts`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        senderId: senderId,
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