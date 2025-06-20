using UnityEditor;
using UnityEngine;
using ScriptableObjects;
using Core;
using System.Linq;
using UnityEditor.Localization;
using System.Collections.Generic;

[CustomEditor(typeof(QuestionsListSO))]
public class QuestionsListSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        QuestionsListSO questionsList = (QuestionsListSO)target;

        if (GUILayout.Button("Load 25 Questions from Localization Tables (Using Naming Pattern)"))
        {
            foreach (var chapter in questionsList.Chapters)
            {
                if (string.IsNullOrEmpty(chapter.LocalizationTableName))
                {
                    Debug.LogWarning("Chapter is missing a LocalizationTableName.");
                    continue;
                }

                var tableCollection = LocalizationEditorSettings.GetStringTableCollection(chapter.LocalizationTableName);

                if (tableCollection == null)
                {
                    Debug.LogError($"Could not find localization table '{chapter.LocalizationTableName}'");
                    continue;
                }

                var sharedEntries = tableCollection.SharedData.Entries.Select(e => e.Key).ToHashSet();

                chapter.Questions.Clear();
                chapter.NameID = $"{chapter.LocalizationTableName}_Name";
                chapter.DescriptionID = $"{chapter.LocalizationTableName}_Description";

                for (int i = 1; i <= 25; i++)
                {
                    string questionKey = $"question_{i}";
                    string quoteKey = $"hint_{i}_1";
                    string backgroundKey = $"hint_{i}_2";

                    if (!sharedEntries.Contains(questionKey) ||
                        !sharedEntries.Contains(quoteKey) ||
                        !sharedEntries.Contains(backgroundKey))
                    {
                        Debug.LogWarning($"Missing base keys for question {i} in table '{chapter.LocalizationTableName}'");
                        continue;
                    }

                    var question = new Question
                    {
                        QuestionTextID = questionKey,
                        QuoteID = quoteKey,
                        HistoricalBackgroundID = backgroundKey
                    };

                    string[] answerSuffixes = { "A", "B", "C", "D" };
                    foreach (var suffix in answerSuffixes)
                    {
                        string answerKey = $"answer_{i}_{suffix}";
                        if (!sharedEntries.Contains(answerKey))
                        {
                            Debug.LogWarning($"Missing answer key '{answerKey}'");
                            continue;
                        }

                        question.Answers.Add(new Answer
                        {
                            AnswerID = answerKey
                        });
                    }

                    chapter.Questions.Add(question);
                }

                Debug.Log($"Loaded {chapter.Questions.Count} questions from '{chapter.LocalizationTableName}'");
            }

            EditorUtility.SetDirty(questionsList);
            AssetDatabase.SaveAssets();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Import Sprite Icons"))
        {
            if (questionsList == null || questionsList.Chapters == null || questionsList.Chapters.Count == 0)
            {
                Debug.LogError("QuestionsList or its Chapters list is null or empty.");
                return;
            }

            for (int chapterIndex = 0; chapterIndex < questionsList.Chapters.Count; chapterIndex++)
            {
                var chapter = questionsList.Chapters[chapterIndex];

                if (chapter == null)
                {
                    Debug.LogWarning($"Chapter at index {chapterIndex} is null. Skipping.");
                    continue;
                }

                if (chapter.Questions == null || chapter.Questions.Count == 0)
                {
                    Debug.LogWarning($"Chapter {chapterIndex} has no questions. Load them first.");
                    continue;
                }

                string spritesPath = $"Assets/[Graphics]/Chapters/{chapterIndex + 1}"; // zakładamy foldery 1-based
                string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { spritesPath });

                if (guids == null || guids.Length == 0)
                {
                    Debug.LogWarning($"No sprites found in folder: {spritesPath}");
                    continue;
                }

                List<Sprite> loadedSprites = guids
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(path => AssetDatabase.LoadAssetAtPath<Sprite>(path))
                    .Where(sprite => sprite != null)
                    .ToList();

                if (loadedSprites.Count == 0)
                {
                    Debug.LogWarning($"Could not load any valid sprite assets from: {spritesPath}");
                    continue;
                }

                for (int questionIndex = 0; questionIndex < chapter.Questions.Count; questionIndex++)
                {
                    var question = chapter.Questions[questionIndex];
                    if (question == null)
                    {
                        Debug.LogWarning($"Question at index {questionIndex} in Chapter {chapterIndex} is null. Skipping.");
                        continue;
                    }

                    string spriteName = $"c{chapterIndex + 1}_{questionIndex + 1}_0";
                    Sprite sprite = loadedSprites.FirstOrDefault(s => s.name == spriteName);

                    if (sprite == null)
                    {
                        Debug.LogWarning($"Sprite '{spriteName}' not found in {spritesPath}");
                        continue;
                    }

                    question.Icon = sprite;
                }

                Debug.Log($"Imported icons for Chapter {chapterIndex + 1}: {chapter.Questions.Count} questions.");
            }

            EditorUtility.SetDirty(questionsList);
            AssetDatabase.SaveAssets();
        }
    }
}
