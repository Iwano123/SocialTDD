import React, { useState } from 'react';
import { postsApi } from '../services/postsApi';
import { ApiError, ErrorCodes } from '../utils/ApiError';
import './CreatePost.css';

function CreatePost({ senderId, onPostCreated }) {
  const [recipientId, setRecipientId] = useState('');
  const [message, setMessage] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);

  const MAX_MESSAGE_LENGTH = 500;
  const MIN_MESSAGE_LENGTH = 1;

  const validateForm = () => {
    setError(null);

    // Validera recipientId
    if (!recipientId || recipientId.trim() === '') {
      setError('Mottagare-ID är obligatoriskt.');
      return false;
    }

    // Validera att recipientId är en giltig GUID
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    if (!guidRegex.test(recipientId.trim())) {
      setError('Mottagare-ID måste vara ett giltigt GUID.');
      return false;
    }

    // Validera att avsändare och mottagare inte är samma
    if (senderId && recipientId.trim().toLowerCase() === senderId.toLowerCase()) {
      setError('Du kan inte posta på din egen tidslinje.');
      return false;
    }

    // Validera message
    const trimmedMessage = message.trim();
    if (!trimmedMessage || trimmedMessage.length === 0) {
      setError('Meddelande är obligatoriskt.');
      return false;
    }

    if (trimmedMessage.length < MIN_MESSAGE_LENGTH) {
      setError(`Meddelande måste vara minst ${MIN_MESSAGE_LENGTH} tecken.`);
      return false;
    }

    if (trimmedMessage.length > MAX_MESSAGE_LENGTH) {
      setError(`Meddelande får inte vara längre än ${MAX_MESSAGE_LENGTH} tecken.`);
      return false;
    }

    return true;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    try {
      setLoading(true);
      setError(null);
      setSuccess(false);

      await postsApi.createPost(
        recipientId.trim(),
        message.trim()
      );

      setSuccess(true);
      setMessage('');
      setRecipientId('');

      // Callback för att notifiera förälder-komponenten
      if (onPostCreated) {
        onPostCreated();
      }

      // Dölj success-meddelandet efter 3 sekunder
      setTimeout(() => {
        setSuccess(false);
      }, 3000);
    } catch (err) {
      if (err instanceof ApiError) {
        switch (err.errorCode) {
          case ErrorCodes.TOKEN_EXPIRED:
            setError('Din session har gått ut. Logga in igen.');
            break;
          case ErrorCodes.NETWORK_ERROR:
            setError('Kunde inte ansluta till servern. Kontrollera din internetanslutning.');
            break;
          case ErrorCodes.TIMEOUT_ERROR:
            setError('Begäran tog för lång tid. Försök igen.');
            break;
          case ErrorCodes.INVALID_RECIPIENT_ID:
            setError('Ogiltigt mottagar-ID.');
            break;
          case ErrorCodes.MESSAGE_TOO_LONG:
            setError('Meddelandet är för långt.');
            break;
          case ErrorCodes.MESSAGE_TOO_SHORT:
            setError('Meddelandet är för kort.');
            break;
          case ErrorCodes.VALIDATION_ERROR:
            setError('Valideringsfel. Kontrollera dina indata.');
            break;
          default:
            setError(err.message || 'Ett fel uppstod vid skapande av inlägg');
        }
      } else {
        setError(err.message || 'Ett fel uppstod vid skapande av inlägg');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleMessageChange = (e) => {
    const newMessage = e.target.value;
    if (newMessage.length <= MAX_MESSAGE_LENGTH) {
      setMessage(newMessage);
      setError(null);
    }
  };

  return (
    <div className="create-post-container">
      <h2 className="create-post-title">Skapa nytt inlägg</h2>
      
      {error && (
        <div className="create-post-error" role="alert">
          {error}
        </div>
      )}

      {success && (
        <div className="create-post-success" role="alert">
          Inlägget skapades framgångsrikt!
        </div>
      )}

      <form onSubmit={handleSubmit} className="create-post-form">
        <div className="create-post-field">
          <label htmlFor="recipientId" className="create-post-label">
            Mottagare-ID (GUID)
          </label>
          <input
            type="text"
            id="recipientId"
            value={recipientId}
            onChange={(e) => {
              setRecipientId(e.target.value);
              setError(null);
            }}
            placeholder="Ange mottagarens GUID"
            className="create-post-input"
            disabled={loading}
            required
          />
        </div>

        <div className="create-post-field">
          <label htmlFor="message" className="create-post-label">
            Meddelande
          </label>
          <textarea
            id="message"
            value={message}
            onChange={handleMessageChange}
            placeholder="Skriv ditt inlägg här..."
            className="create-post-textarea"
            rows="5"
            disabled={loading}
            required
          />
          <div className="create-post-character-count">
            {message.length} / {MAX_MESSAGE_LENGTH} tecken
          </div>
        </div>

        <button
          type="submit"
          className="create-post-button"
          disabled={loading || !senderId}
        >
          {loading ? 'Skapar...' : 'Skapa inlägg'}
        </button>
      </form>
    </div>
  );
}

export default CreatePost;