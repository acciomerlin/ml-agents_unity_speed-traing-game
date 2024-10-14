using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 导入SceneManagement命名空间
using TMPro; // 导入TextMeshPro命名空间
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public MoveToGoalAgent agent;
    public TextMeshProUGUI resultText; // 结果显示文本

    public GameObject startPanel;
    public GameObject endPanel;

    public Slider speedSlider;
    public Slider roundSlider;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI scoreText; // 比分显示文本
    public TextMeshProUGUI roundInfoText; // 回合信息显示文本
    public TextMeshProUGUI speedShowTxt; // 速度显示文本

    public int playerClicks = 0;
    public int penguinScore = 0;
    public int totalRounds = 10;
    public int currentRound = 0;

    void Start()
    {
        // 初始化并显示开始面板
        ShowStartPanel();

        // 设置Slider初始值并更新显示
        speedSlider.value = 10;
        roundSlider.value = 10;
        OnSpeedSliderChanged(speedSlider.value);
        OnRoundSliderChanged(roundSlider.value);

        // 初始化比分和回合信息
        UpdateScoreText();
        UpdateRoundInfoText();
    }

    private void Update()
    {
        // 每帧更新比分和回合信息
        UpdateScoreText();
        UpdateRoundInfoText();
    }

    public void PlayerClick()
    {
        // 玩家点击，增加点击次数和当前回合，结束当前回合并检查游戏状态
        playerClicks++;
        currentRound++;
        agent.EndEpisode();
        CheckGameStatus();
    }

    public void AgentTouch()
    {
        // 企鹅到达目标，增加企鹅得分和当前回合，结束当前回合并检查游戏状态
        penguinScore++;
        currentRound++;
        agent.EndEpisode();
        CheckGameStatus();
    }

    public void CheckGameStatus()
    {
        // 检查当前回合是否达到总回合数，若是则结束游戏
        if (currentRound >= totalRounds)
        {
            EndGame(playerClicks, penguinScore);
        }
    }

    void EndGame(int playerClicks, int penguinScore)
    {
        // 根据得分判断胜负，并显示结果和结束面板
        if (playerClicks > penguinScore)
        {
            resultText.text = "You Win!";
        }
        else if (playerClicks < penguinScore)
        {
            resultText.text = "Penguin Wins!";
        }
        else
        {
            resultText.text = "Draw!";
        }
        agent.StopAgent();
        ShowEndPanel();
    }

    public void StartGame()
    {
        // 开始游戏，隐藏开始面板和滑块
        resultText.text = "";
        startPanel.SetActive(false);
        HideSliders();
        agent.isGameActive = true;
        agent.OnEpisodeBegin(); // 开始新的Episode
    }

    public void RestartGame()
    {
        // 重启游戏，重置计数并开始新的Episode
        endPanel.SetActive(false);
        HideSliders();
        agent.isGameActive = true;
        playerClicks = 0;
        penguinScore = 0;
        currentRound = 0;
        resultText.text = "";
        agent.OnEpisodeBegin(); // 开始新的Episode
    }

    public void QuitGame()
    {
        // 退出游戏
        Application.Quit();
    }

    void ShowStartPanel()
    {
        // 显示开始面板和滑块
        ShowSliders();
        startPanel.SetActive(true);
        endPanel.SetActive(false);
    }

    void ShowEndPanel()
    {
        // 显示结束面板和滑块
        ShowSliders();
        endPanel.SetActive(true);
    }

    public void OnSpeedSliderChanged(float value)
    {
        // 当速度滑块值变化时，更新速度并重置计数
        playerClicks = 0;
        penguinScore = 0;
        currentRound = 0;
        int intValue = Mathf.RoundToInt(value / 5) * 5;
        speedSlider.value = intValue;
        speedText.text = "Penguin's Speed: " + intValue;
        speedShowTxt.text = speedText.text;
        agent.SetSpeed(intValue);
    }

    public void OnRoundSliderChanged(float value)
    {
        // 当回合滑块值变化时，更新总回合数并重置计数
        playerClicks = 0;
        penguinScore = 0;
        currentRound = 0;
        int intValue = Mathf.RoundToInt(value / 5) * 5;
        roundSlider.value = intValue;
        roundText.text = "Rounds: " + intValue;
        totalRounds = intValue;
    }

    private void ShowSliders()
    {
        // 显示滑块和对应的文本
        speedSlider.gameObject.SetActive(true);
        roundSlider.gameObject.SetActive(true);
        speedText.gameObject.SetActive(true);
        roundText.gameObject.SetActive(true);
    }

    private void HideSliders()
    {
        // 隐藏滑块和对应的文本
        speedSlider.gameObject.SetActive(false);
        roundSlider.gameObject.SetActive(false);
        speedText.gameObject.SetActive(false);
        roundText.gameObject.SetActive(false);
    }

    private void UpdateScoreText()
    {
        // 更新比分显示
        scoreText.text = "You: " + playerClicks + " vs Penguin: " + penguinScore;
    }

    private void UpdateRoundInfoText()
    {
        // 更新回合信息显示
        roundInfoText.text = "Round: " + currentRound + "/" + totalRounds;
    }
}