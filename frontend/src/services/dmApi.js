const API_BASE_URL = 'http://localhost:5000/api';

export const dmApi = {
  // Skicka ett direktmeddelande
  async sendDirectMessage(senderId, recipientId, message) {
    const response = await fetch(`${API_BASE_URL}/directmessages`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        senderId,
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
    const response = await fetch(`${API_BASE_URL}/directmessages/received/${userId}`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || error.message || 'Kunde inte hämta meddelanden');
    }

    return await response.json();
  },

  // Markera ett meddelande som läst
  async markAsRead(messageId) {
    const response = await fetch(`${API_BASE_URL}/directmessages/${messageId}/read`, {
      method: 'PUT',
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || error.message || 'Kunde inte markera meddelande som läst');
    }
  },
};
