public static class PathManager
{
    #region FilePath
    //Json
    public const string storyJsonFilePath = "Assets/Resources/StoryData/StoryData_1.json";
    public const string itemJsonFilePath = "Assets/Resources/StoryData/ItemData_1.json";
    
    //CSV
    public const string csvPath = "Assets/Resources/StoryData/CSV/";
    public const string nodeFileName = csvPath + "Node.csv";
    public const string choiceFileName = csvPath + "Choice.csv";
    public const string choiceConditionFileName = csvPath + "Choicecondition.csv";
    public const string rewardFileName = csvPath + "Reward.csv";
    public const string cgFileName = csvPath + "Cg.csv";
    public const string characterSpriteFileName = csvPath + "CharacterSprite.csv";
    public const string dialogSoundFileName = csvPath + "DialogSound.csv";
    public const string chapterDataFileName = csvPath + "ChapterData.csv";
    public const string itemDataFileName = csvPath + "ItemData.csv";
    //Save
    public const string saveFileName = "saveFile.json";
    public const string endingSaveFileName = "endingSaveFile.json";
    #endregion

    #region SceneName
    public const string TitleSceneName = "Title";
    public const string InGameSceneName = "InGame";
    #endregion
    #region Localization
    public const string dialogTableName = "DialogLocalizationTable";
    public const string affinityTextName = "TEXT_AFFINITY";
    #endregion
}