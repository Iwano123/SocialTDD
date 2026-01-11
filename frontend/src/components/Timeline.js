import React, { useState, useEffect, useCallback } from 'react';
import { postsApi } from '../services/postsApi';
import { ApiError, ErrorCodes } from '../utils/ApiError';
import './Timeline.css';

function Timeline({ userId }) {
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchTimeline = useCallback(async () => {
    if (!userId) {
      return;
    }

    try {
      setLoading(true);
      setError(null);
      const timelinePosts = await postsApi.getTimeline();
      // Sortera inlägg så att senaste kommer först (kronologisk ordning)
      const sortedPosts = timelinePosts.sort((a, b) => {
        return new Date(b.createdAt) - new Date(a.createdAt);
      });
      setPosts(sortedPosts);
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
          case ErrorCodes.INVALID_USER_ID:
            setError('Ogiltigt användar-ID.');
            break;
          default:
            setError(err.message || 'Kunde inte hämta tidslinje');
        }
      } else {
        setError(err.message || 'Kunde inte hämta tidslinje');
      }
    } finally {
      setLoading(false);
    }
  }, [userId]);

  useEffect(() => {
    if (userId) {
      fetchTimeline();
    }
  }, [userId, fetchTimeline]);

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('sv-SE', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  if (loading && posts.length === 0) {
    return (
      <div className="timeline">
        <div className="timeline-header">
          <h3>Tidslinje</h3>
        </div>
        <div className="loading">
          <span className="loading-spinner"></span>
          <span>Laddar tidslinje...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="timeline">
      <div className="timeline-header">
        <h3>Tidslinje</h3>
        <button
          onClick={fetchTimeline}
          className="timeline-refresh-button"
          disabled={loading}
          title="Uppdatera tidslinje"
          aria-label="Uppdatera tidslinje"
        >
          <span className={loading ? 'refresh-icon spinning' : 'refresh-icon'}>⟳</span>
        </button>
      </div>
      
      {error && (
        <div className="error-message" role="alert">
          <span className="error-icon">⚠️</span>
          <span className="error-text">{error}</span>
          <button onClick={fetchTimeline} className="error-retry-button">
            Försök igen
          </button>
        </div>
      )}

      {posts.length === 0 && !loading && !error ? (
        <div className="empty-message">
          <p>Inga inlägg att visa i tidslinjen.</p>
          <p className="empty-hint">Skapa ett inlägg eller vänta på att någon postar på din tidslinje.</p>
        </div>
      ) : (
        <div className="timeline-posts">
          {posts.map((post) => (
            <div key={post.id} className="post-item">
              <div className="post-header">
                <span className="post-sender">Från: {post.senderId}</span>
                <span className="post-date">{formatDate(post.createdAt)}</span>
              </div>
              <div className="post-message">{post.message}</div>
              {post.recipientId && (
                <div className="post-recipient">Till: {post.recipientId}</div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default Timeline;