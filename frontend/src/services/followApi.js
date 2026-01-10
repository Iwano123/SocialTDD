const API_BASE_URL = 'http://localhost:5000/api';

export const followApi = {
  // Följa en användare
  async followUser(followerId, followingId) {
    const response = await fetch(`${ API_BASE_URL }/follow`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        followerId,
        followingId,
      }),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Kunde inte följa användare');
    }

    return await response.json();
  },

  // Avfölja en användare
  async unfollowUser(followerId, followingId) {
    const response = await fetch(`${ API_BASE_URL }/follow/${ followerId }/${ followingId }`, {
      method: 'DELETE',
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Kunde inte avfölja användare');
    }
  },

  // Hämta följare för en användare
  async getFollowers(userId) {
    const response = await fetch(`${ API_BASE_URL }/follow/followers/${ userId }`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Kunde inte hämta följare');
    }

    return await response.json();
  },

  // Hämta följda användare för en användare
  async getFollowing(userId) {
    const response = await fetch(`${ API_BASE_URL }/follow/following/${ userId }`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Kunde inte hämta följda användare');
    }

    return await response.json();
  },
};