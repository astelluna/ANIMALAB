using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ChapterThumbnail
{
    public string key;
    public Sprite thumbnail;
}

[CreateAssetMenu(menuName = "Background/ThumbnailDatabaseSO")]
public class ChapterThumbnailDatabaseSO : ScriptableObject
{
    //이거도 자동화 해놓자. 챕터
    [field:SerializeField] public List<ChapterThumbnail> chapterThumbnails { get; private set; }

    public Sprite GetThumbnailSprite(string spriteId)
    {
        ChapterThumbnail data = chapterThumbnails.Find(x => x.key == spriteId);

        if (data == null)
        {
            Debug.LogError($"썸네일 테이블에 없는 key: {spriteId}");
            return null;
        }

        return data.thumbnail;
    }

}