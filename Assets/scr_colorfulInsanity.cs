using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CodeExtension;
using UnityEngine;

using Random = UnityEngine.Random;

public class scr_colorfulInsanity : MonoBehaviour {
    public KMAudio BombAudio;
    public KMBombInfo BombInfo;
    public KMBombModule BombModule;
    public KMSelectable[] ModuleButtons;
    public KMSelectable ModuleSelect;
    public KMRuleSeedable RuleSeedable;
    public Texture2D[] TexPatternsA;
    public Texture2D[] TexPatternsB;

    Color[] selectColors = {
        Color.red,
        new Color32(255, 127, 39, 255), //Orange
		Color.yellow,
        Color.green,
        Color.cyan,
        new Color32(0, 162, 232, 255), //Azure
		Color.blue,
        Color.magenta,
        new Color32(163, 73, 164, 255) //Purple
	};

    int[] chosenPatterns = Enumerable.Repeat(-1, 35).ToArray();
    int[,] chosenColors = Enumerable.Repeat(-1, 70).ToArray().ToArray2D(2, 35);
    int[] specialButtons = Enumerable.Repeat(-1, 4).ToArray();
    List<int> correctPatterns;
    string correctColors = "";

    MonoRandom rnd;
    List<int> pickedValues = new List<int>();

    delegate bool checkSame(int[] w, int[,] x, int y, int[] z);

    int[] nowPatterns = Enumerable.Range(0, 25).ToArray();
    readonly string[] colorLetters = { "", "R", "O", "Y", "G", "C", "A", "B", "M", "P" };
    readonly string[,] nowColors = new string[9, 9];
    List<int> correctTotal = new List<int>();
    List<int> pressedButtons = new List<int>();

    bool moduleSolved;

    static int moduleIdCounter = 1;
    int moduleId;

    void Start() {
        moduleId = moduleIdCounter++;
        rnd = RuleSeedable.GetRNG();
        Debug.LogFormat(@"[Colorful Insanity #{0}] Using rule seed: {1}", moduleId, rnd.Seed);
        nowPatterns = rnd.ShuffleFisherYates(nowPatterns);

        for (int i = 0; i < 9; i++) {
            for (int j = 0; j < 9; j++) {
                nowColors[i, j] = new[] {
                    colorLetters[rnd.Next(10)],
                    colorLetters[rnd.Next(10)],
                    colorLetters[rnd.Next(10)]
                }.Distinct().ToArray().ToStringElements("");
            }
        }

        for (int i = 0; i < specialButtons.Length; i++) {
            specialButtons[i] = (i == 0) ? Random.Range(0, ModuleButtons.Length) : ChooseUnique(specialButtons.Take(i).ToArray(), ModuleButtons.Length);
        }

        SetPatterns(specialButtons[0], GetPatterns());
        SetPatterns(specialButtons[1], new[] {
            chosenPatterns[specialButtons[0]],
            chosenColors[0, specialButtons[0]],
            chosenColors[1, specialButtons[0]]
        });

        var checkPatterns = new int[3];

        do {
            checkPatterns = GetPatterns();
        } while (Enumerable.Range(0, 2).Any(x => new[] {
            chosenPatterns[specialButtons[x]],
            chosenColors[0, specialButtons[x]],
            chosenColors[1, specialButtons[x]]
        }.SequenceEqual(checkPatterns)));

        SetPatterns(specialButtons[2], checkPatterns);
        SetPatterns(specialButtons[3], new[] {
            chosenPatterns[specialButtons[2]],
            chosenColors[1, specialButtons[2]],
            chosenColors[0, specialButtons[2]]
        });

        Debug.LogFormat(@"[Colorful Insanity #{0}] 1st pair is in: {1}", moduleId, specialButtons.Skip(2).Select(x => x.ToCoord(7)).ToStringElements(", "));
        Debug.LogFormat(@"[Colorful Insanity #{0}] 2nd pair is in: {1}", moduleId, specialButtons.Take(2).Select(x => x.ToCoord(7)).ToStringElements(", "));

        checkSame[] getSame = {
            ((w, x, y, z) => Enumerable.Range(0, y).Any(a => (new[] {
                w[a],
                x[0, a],
                x[1, a]
            }.SequenceEqual(z)) || (new[] {
                w[a],
                x[1, a],
                x[0, a]
            }.SequenceEqual(z)))),
            ((w, x, y, z) => Enumerable.Range(0, y).Any(a => new[] {
                w[specialButtons[a]],
                x[0, specialButtons[a]],
                x[1, specialButtons[a]]
            }.SequenceEqual(z) || new[] {
                w[specialButtons[a]],
                x[1, specialButtons[a]],
                x[0, specialButtons[a]]
            }.SequenceEqual(z)))
        };

        for (int i = 0; i < ModuleButtons.Length; i++) {
            if (!specialButtons.Contains(i)) {
                do {
                    checkPatterns = GetPatterns();
                } while (getSame[0](chosenPatterns, chosenColors, i, checkPatterns) || getSame[1](chosenPatterns, chosenColors, specialButtons.Length, checkPatterns));

                SetPatterns(i, checkPatterns);
            }

            AssignPattern(i);
            int j = i;
            ModuleButtons[i].OnInteract += delegate() {
                OnButtonPress(j);

                return false;
            };
        }

        var patternPos = Array.IndexOf(nowPatterns, chosenPatterns[specialButtons[2]]);
        correctPatterns = GetNearPatterns(patternPos);
        Debug.LogFormat(@"[Colorful Insanity #{0}] In the table, 1st pair's pattern is on: {1}", moduleId, patternPos.ToCoord(5));
        Debug.LogFormat(@"[Colorful Insanity #{0}] In the table, correct patterns are on: {1}", moduleId, correctPatterns.Select(x => Array.IndexOf(nowPatterns, x).ToCoord(5)).Join(", "));
        correctColors = nowColors[chosenColors[0, specialButtons[0]], chosenColors[1, specialButtons[0]]];
        string[] colorNames = { "", "Red", "Orange", "Yellow", "Green", "Cyan", "Azure", "Blue", "Magenta", "Purple" };
        Debug.LogFormat(@"[Colorful Insanity #{0}] Correct colors are: {1}", moduleId, (!correctColors.Equals("")) ? correctColors.Select(x => colorNames[Array.IndexOf(colorLetters, x.ToString())]).Join(", ") : "Any");

        for (int i = 0; i < ModuleButtons.Length; i++) {
            if (correctPatterns.Contains(chosenPatterns[i]) && (correctColors.Equals("") || Enumerable.Range(0, 2).Any(x => Enumerable.Range(0, correctColors.Length).Select(y => Array.IndexOf(colorLetters, correctColors[y].ToString()) - 1).ToArray().Contains(chosenColors[x, i])))) {
                correctTotal.Add(i);
            }
        }

        if (correctTotal.Count == 0) {
            correctTotal.AddRange(specialButtons);
            Debug.LogFormat(@"[Colorful Insanity #{0}] None of the buttons match. Buttons to press are the initial pairs.", moduleId);
        } else {
            Debug.LogFormat(@"[Colorful Insanity #{0}] Buttons to press are on: {1}", moduleId, correctTotal.Select(x => x.ToCoord(7)).Join(", "));
        }
    }

    int[] GetPatterns() {
        var choosePatterns = new int[3];
        choosePatterns[0] = Random.Range(0, TexPatternsA.Length);
        choosePatterns[1] = Random.Range(0, selectColors.Length);
        choosePatterns[2] = ChooseUnique(new[] { choosePatterns[1] }, selectColors.Length);

        return choosePatterns;
    }

    void SetPatterns(int numAssign, int[] assignPatterns) {
        chosenPatterns[numAssign] = assignPatterns[0];
        chosenColors[0, numAssign] = assignPatterns[1];
        chosenColors[1, numAssign] = assignPatterns[2];
    }

    int ChooseUnique(int[] checkVal, int maxVal) {
        var nowVal = 0;

        do {
            nowVal = Random.Range(0, maxVal);
        } while (checkVal.Any(x => x == nowVal));

        return nowVal;
    }

    int ChooseUniqueRND(int maxVal) {
        var nowVal = 0;

        do {
            nowVal = rnd.Next(maxVal);
        } while (pickedValues.Contains(nowVal));

        pickedValues.Add(nowVal);

        return nowVal;
    }

    void AssignPattern(int buttonAssign) {
        var childComp = ModuleButtons[buttonAssign].transform.GetChild(0).GetComponent<Renderer>();
        childComp.material.mainTexture = TexPatternsA[chosenPatterns[buttonAssign]];
        childComp.material.color = selectColors[chosenColors[0, buttonAssign]];
        childComp = ModuleButtons[buttonAssign].transform.GetChild(1).GetComponent<Renderer>();
        childComp.material.mainTexture = TexPatternsB[chosenPatterns[buttonAssign]];
        childComp.material.color = selectColors[chosenColors[1, buttonAssign]];
    }

    List<int> GetNearPatterns(int nowCheck) {
        var retPatterns = new List<int>();
        var posPatterns = new[] { -1, 1, -5, 5 };

        for (int i = 0; i < 4; i++) {
            var nowPos = posPatterns[i];

            if (i < 2) {
                if (((nowCheck % 5) + nowPos).IsInRange(0, 4)) {
                    retPatterns.Add(nowPatterns[nowCheck + nowPos]);
                }
            } else {
                var toCheck = nowCheck + nowPos;

                if (toCheck.IsInRange(0, 24)) {
                    retPatterns.Add(nowPatterns[toCheck]);
                }
            }
        }

        return retPatterns;
    }

    void OnButtonPress(int buttonPressed) {
        BombAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        ModuleSelect.AddInteractionPunch(0.25f);

        if (moduleSolved || pressedButtons.Contains(buttonPressed)) {
            return;
        }

        var buttonComp = ModuleButtons[buttonPressed].GetComponent<Renderer>();
        var pressedCoord = buttonPressed.ToCoord(7);

        if (correctTotal.Contains(buttonPressed)) {
            buttonComp.material.color = Color.white;

            if (pressedButtons.Count + 1 < correctTotal.Count) {
                pressedButtons.Add(buttonPressed);
                Debug.LogFormat(@"[Colorful Insanity #{0}] You pressed button on {1} which is correct. {2} more left.", moduleId, pressedCoord, correctTotal.Count - pressedButtons.Count);
            } else {
                BombModule.HandlePass();
                moduleSolved = true;
                Debug.LogFormat(@"[Colorful Insanity #{0}] Module solved!", moduleId);
            }
        } else {
            BombModule.HandleStrike();
            buttonComp.material.color = Color.gray;
            Debug.LogFormat(@"[Colorful Insanity #{0}] You pressed button on {1} which is incorrect.", moduleId, pressedCoord);
        }
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press A1 B3 C5... (column [A to G] and row [1 to 5] to press)";
    #pragma warning restore 414

    KMSelectable[] ProcessTwitchCommand(string command) {
        command = command.ToLowerInvariant().Trim();

        if (Regex.IsMatch(command, @"^press +[a-g1-5^, |&]+$")) {
            command = command.Substring(6).Trim();

            var presses = command.Split(new[] { ',', ' ', '|', '&' }, StringSplitOptions.RemoveEmptyEntries);
            var pressList = new List<KMSelectable>();

            for (int i = 0; i < presses.Length; i++) {
                if (Regex.IsMatch(presses[i], @"^[a-g][1-5]$")) {
                    var setPress = (presses[i][0] - 'a') + (7 * (presses[i][1] - '1'));
                    pressList.Add(ModuleButtons[setPress]);
                }
            }

            return (pressList.Count > 0) ? pressList.ToArray() : null;
        }

        return null;
    }
}