import { authenticatedFetch } from '../utils/apiClient';

const API_BASE_URL = 'http://localhost:5000/api';

export const dmApi = {
  // Skicka ett direktmeddelande
  async sendDirectMessage(recipientId, message) {
    const response = await authenticatedFetch(`${API_BASE_URL}/directmessages`, {
      method: 'POST',
      body: JSON.stringify({
        recipientId,
        message,
      }),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || error.message || 'Kunde inte skicka meddelande');
    }

    return await response.json();
  },

  // Hämta mottagna meddelanden för en användare
  async getReceivedMessages(userId) {
    const response = await authenticatedFetch(`${API_BASE_URL}/directmessages/received`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || error.message || 'Kunde inte hämta meddelanden');
    }

    return await response.json();
  },

  // Markera ett meddelande som läst
  async markAsRead(messageId) {
    const response = await authenticatedFetch(`${API_BASE_URL}/directmessages/${messageId}/read`, {
      method: 'PUT',
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || error.message || 'Kunde inte markera meddelande som läst');
    }
  },
};
