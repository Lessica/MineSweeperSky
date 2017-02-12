using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ScoreManager : MonoBehaviour
{
	void Awake()
	{
		
	}

	void Start()
	{
		Social.localUser.Authenticate(HandleAuthenticated);
	}

	private void HandleAuthenticated(bool success)
	{
		Debug.Log("*** HandleAuthenticated: success = " + success);
		if (success) {
			Social.localUser.LoadFriends(HandleFriendsLoaded);
			Social.LoadAchievements(HandleAchievementsLoaded);
			Social.LoadAchievementDescriptions(HandleAchievementDescriptionsLoaded);
		}
	}

	private void HandleFriendsLoaded(bool success)
	{
		Debug.Log("*** HandleFriendsLoaded: success = " + success);
		foreach (IUserProfile friend in Social.localUser.friends) {
			Debug.Log("*   friend = " + friend.ToString());
		}
	}

	private void HandleAchievementsLoaded(IAchievement[] achievements)
	{
		Debug.Log("*** HandleAchievementsLoaded");
		foreach (IAchievement achievement in achievements) {
			Debug.Log("*   achievement = " + achievement.ToString());
		}
	}

	private void HandleAchievementDescriptionsLoaded(IAchievementDescription[] achievementDescriptions)
	{
		Debug.Log("*** HandleAchievementDescriptionsLoaded");
		foreach (IAchievementDescription achievementDescription in achievementDescriptions) {
			Debug.Log("*   achievementDescription = " + achievementDescription.ToString());
		}
	}

	public void ReportProgress(string achievementId, double progress)
	{
		if (Social.localUser.authenticated) {
			Social.ReportProgress(achievementId, progress, HandleProgressReported);
		}
	}

	private void HandleProgressReported(bool success)
	{
		Debug.Log("*** HandleProgressReported: success = " + success);
	}

	public void ShowAchievements()
	{
		if (Social.localUser.authenticated) {
			Social.ShowAchievementsUI();
		}
	}

	// leaderboard

	public void ReportScore(string leaderboardId, long score)
	{
		if (Social.localUser.authenticated) {
			Social.ReportScore(score, leaderboardId, HandleScoreReported);
		}
	}

	public void HandleScoreReported(bool success)
	{
		Debug.Log("*** HandleScoreReported: success = " + success);

	}

	public void ShowLeaderboard()
	{
		if (Social.localUser.authenticated) {
			Social.ShowLeaderboardUI();
		}
	}
}
