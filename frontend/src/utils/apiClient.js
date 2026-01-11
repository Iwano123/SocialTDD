// Utility function för att hämta token från localStorage
const getAuthToken = () => {
  return localStorage.getItem('token');
};

// Helper function för att göra autentiserade API-anrop
export const authenticatedFetch = async (url, options = {}) => {
  const token = getAuthToken();
  
  const headers = {
    'Content-Type': 'application/json',
    ...options.headers,
  };

  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const response = await fetch(url, {
    ...options,
    headers,
  });

  return response;
};
