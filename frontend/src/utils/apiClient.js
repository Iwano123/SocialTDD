import { ApiError, ErrorCodes } from './ApiError';

const API_TIMEOUT = 10000; // 10 sekunder

// Utility function för att hämta token från localStorage
const getAuthToken = () => {
  return localStorage.getItem('token');
};

const createTimeoutPromise = (timeoutMs) => {
  return new Promise((_, reject) => {
    setTimeout(() => reject(new ApiError(
      ErrorCodes.TIMEOUT_ERROR,
      'Begäran tog för lång tid. Kontrollera din internetanslutning.',
      null
    )), timeoutMs);
  });
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

  try {
    // Lägg till timeout
    const fetchPromise = fetch(url, {
      ...options,
      headers,
    });

    const response = await Promise.race([
      fetchPromise,
      createTimeoutPromise(API_TIMEOUT)
    ]);

    // Hantera 401 Unauthorized
    if (response.status === 401) {
      localStorage.removeItem('token');
      throw new ApiError(
        ErrorCodes.TOKEN_EXPIRED,
        'Din session har gått ut. Logga in igen.',
        401
      );
    }

    return response;
  } catch (error) {
    // Hantera nätverksfel
    if (error instanceof ApiError) {
      throw error;
    }
    
    // Nätverksfel (ingen anslutning, timeout, etc.)
    if (error.name === 'TypeError' || error.message.includes('fetch')) {
      throw new ApiError(
        ErrorCodes.NETWORK_ERROR,
        'Kunde inte ansluta till servern. Kontrollera din internetanslutning.',
        null
      );
    }
    
    throw error;
  }
};

// Helper för att hantera API-svar
export const handleApiResponse = async (response) => {
  if (!response.ok) {
    let errorData;
    try {
      errorData = await response.json();
    } catch {
      // Om vi inte kan parsa JSON, använd status text
      throw new ApiError(
        ErrorCodes.INTERNAL_SERVER_ERROR,
        `Serverfel: ${response.statusText}`,
        response.status
      );
    }

    // Standardiserad felstruktur från backend
    const errorCode = errorData.errorCode || ErrorCodes.INTERNAL_SERVER_ERROR;
    const message = errorData.message || errorData.error || 'Ett fel uppstod';
    
    throw new ApiError(errorCode, message, response.status, errorData.details);
  }

  return await response.json();
};