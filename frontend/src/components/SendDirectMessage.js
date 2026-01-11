import React, { useState } from 'react';
import { dmApi } from '../services/dmApi';
import { ApiError, ErrorCodes } from '../utils/ApiError';
import UserSearch from './UserSearch';
import './SendDirectMessage.css';

function SendDirectMessage({ senderId, onMessageSent }) {
  const [recipientId, setRecipientId] = useState('');
  const [message, setMessage] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);

  const MAX_MESSAGE_LENGTH = 500;
  const MIN_MESSAGE_LENGTH = 1;

  const handleUserSelect = (user) => {
    setRecipientId(user.id);
    setError(null);
  };

  const validateForm = () => {
    setError(null);

    // Validera recipientId
    if (!recipientId || recipientId.trim() === '') {
      setError('Mottagare är obligatoriskt.');
      return false;
    }

    // Validera att recipientId är en giltig GUID
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    if (!guidRegex.test(recipientId.trim())) {
      setError('Ogiltigt användar-ID.');
      return false;
    }

    // Validera att avsändare och mottagare inte är samma
    if (senderId && recipientId.trim().toLowerCase() === senderId.toLowerCase()) {
      setError('Du kan inte skicka meddelande till dig själv.');
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

      await dmApi.sendDirectMessage(recipientId.trim(), message.trim());

      setSuccess(true);
      setMessage('');
      setRecipientId('');

      // Callback för att notifiera förälder-komponenten
      if (onMessageSent) {
        onMessageSent();
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
            setError(err.message || 'Ett fel uppstod vid skickande av meddelande');
        }
      } else {
        setError(err.message || 'Ett fel uppstod vid skickande av meddelande');
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
    <div className="send-dm-container">
      <h2 className="send-dm-title">Skicka direktmeddelande</h2>
      
      {error && (
        <div className="send-dm-error" role="alert">
          {error}
        </div>
      )}

      {success && (
        <div className="send-dm-success" role="alert">
          Meddelandet skickades framgångsrikt!
        </div>
      )}

      <form onSubmit={handleSubmit} className="send-dm-form">
        <div className="send-dm-field">
          <label htmlFor="recipientId" className="send-dm-label">
            Mottagare
          </label>
          <UserSearch
            onUserSelect={handleUserSelect}
            placeholder="Sök efter användare..."
            excludeUserId={senderId}
          />
        </div>

        <div className="send-dm-field">
          <label htmlFor="message" className="send-dm-label">
            Meddelande
          </label>
          <textarea
            id="message"
            value={message}
            onChange={handleMessageChange}
            placeholder="Skriv ditt meddelande här..."
            className="send-dm-textarea"
            rows="5"
            disabled={loading}
            required
          />
          <div className="send-dm-character-count">
            {message.length} / {MAX_MESSAGE_LENGTH} tecken
          </div>
        </div>

        <button
          type="submit"
          className="send-dm-button"
          disabled={loading || !senderId}
        >
          {loading ? 'Skickar...' : 'Skicka meddelande'}
        </button>
      </form>
    </div>
  );
}

export default SendDirectMessage;
