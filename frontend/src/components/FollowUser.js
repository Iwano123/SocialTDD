import React, { useState, useEffect } from 'react';
import { followApi } from '../services/followApi';
import './FollowUser.css';

function FollowUser({ followerId, followingId, onFollowChange }) {
  const [isFollowing, setIsFollowing] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [checkingStatus, setCheckingStatus] = useState(true);

  // Kontrollera om användaren redan följs
  useEffect(() => {
    const checkFollowStatus = async () => {
      try {
        setCheckingStatus(true);
        const following = await followApi.getFollowing(followerId);
        const isCurrentlyFollowing = following.some(
          (follow) => follow.followingId === followingId
        );
        setIsFollowing(isCurrentlyFollowing);
      } catch (err) {
        setError(err.message);
      } finally {
        setCheckingStatus(false);
      }
    };

    if (followerId && followingId) {
      checkFollowStatus();
    }
  }, [followerId, followingId]);

  const handleFollow = async () => {
    try {
      setLoading(true);
      setError(null);
      await followApi.followUser(followingId);
      setIsFollowing(true);
      if (onFollowChange) {
        onFollowChange(true);
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleUnfollow = async () => {
    try {
      setLoading(true);
      setError(null);
      await followApi.unfollowUser(followingId);
      setIsFollowing(false);
      if (onFollowChange) {
        onFollowChange(false);
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  if (checkingStatus) {
    return <div className="follow-user-loading">Kontrollerar status...</div>;
  }

  return (
    <div className="follow-user">
      {error && <div className="follow-user-error">{error}</div>}
      {isFollowing ? (
        <button
          onClick={handleUnfollow}
          disabled={loading}
          className="follow-user-button unfollow-button"
        >
          {loading ? 'Avföljer...' : 'Avfölj'}
        </button>
      ) : (
        <button
          onClick={handleFollow}
          disabled={loading}
          className="follow-user-button follow-button"
        >
          {loading ? 'Följer...' : 'Följ'}
        </button>
      )}
    </div>
  );
}

export default FollowUser;