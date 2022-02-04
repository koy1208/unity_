using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class JsonSingleton : MonoBehaviour
{
    static public JsonSingleton _instance = null;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

    }

    [System.Serializable]
    public class SaveJsonFile
    {
        public string ID;
        public string PW;

        public List<CharacterManager.CharacterInfo> HasCharacterList = new List<CharacterManager.CharacterInfo>();
      
        public int[] InPartyCharacterArray = new int[4];

        public List<Equipment_Base.Equipment_Info> HasEquipment = new List<Equipment_Base.Equipment_Info>();
        public List<ItemManager.ItemInfo> HasItem = new List<ItemManager.ItemInfo>();

        public List<string> ClearStageList = new List<string>();

        public SaveJsonFile() { }

        public SaveJsonFile(string id, string pw)
        {
            ID = id;
            PW = pw;
            

            for (int i = 0;  i < InPartyCharacterArray.Length; i++)
            {
                InPartyCharacterArray[i] = -1;
            }

            AddCharacter("Knite");
            AddCharacterAtParty(HasCharacterList[0]);

            AddCharacter("Archor");

        }

        public bool CheckPW(string pw)
        {
            if (PW == pw)
                return true;

            return false;
        }
     
        ///////////////////////////////////////////////
    

        public void AddCharacter(string name)
        {
            var newCharacter = new CharacterManager.CharacterInfo(name);
            HasCharacterList.Add(newCharacter);
        }
        

        private void AddCharacterAtParty(CharacterManager.CharacterInfo ChInfo)
        {
            for (int i = 0; i < 4; i++)
            {
                if (InPartyCharacterArray[i] == -1)
                {
                    InPartyCharacterArray[i] = ChInfo.ID;
                    ChInfo.IsParty = true;
                    return;
                }
            }
        }

        public void AddCharacterAtParty(CharacterManager.CharacterInfo ChInfo, int idx)
        {
            for(int i = 0; i < InPartyCharacterArray.Length; i++)
            {
                //
                if (InPartyCharacterArray[i] == -1)
                    continue;

                var inpartyCharacter = Player._instance.PlayerInfo.FindCharacterAtID(InPartyCharacterArray[i]);

                if (inpartyCharacter.CharacterName == null ||
                    inpartyCharacter.CharacterName == "")
                    continue;
                
                if (inpartyCharacter.ID == ChInfo.ID)
                    return;
            }

            if (InPartyCharacterArray[idx] != -1)
            {
                FindCharacterAtID(InPartyCharacterArray[idx]).IsParty = false;
            }

            InPartyCharacterArray[idx] = ChInfo.ID;
            FindCharacterAtID(InPartyCharacterArray[idx]).IsParty = true;
            ChInfo.IsParty = true;
            return;
        }

        public void RemoveCharacterAtParty(CharacterManager.CharacterInfo ChInfo)
        {
            for(int i = 0; i <InPartyCharacterArray.Length; i++)
            {
                if (InPartyCharacterArray[i] == -1)
                    continue;

                var inpartyCharacter = Player._instance.PlayerInfo.FindCharacterAtID(InPartyCharacterArray[i]);

                if (inpartyCharacter.CharacterName == null ||
                   inpartyCharacter.CharacterName == "")
                    continue;

                if (InPartyCharacterArray[i] == ChInfo.ID)
                {
                    FindCharacterAtID(ChInfo.ID).IsParty = false;
                    InPartyCharacterArray[i] = -1;
                    break;
                }
            }
        }

        public void GetHasCharacterList(out List<CharacterManager.CharacterInfo> info)
        {
            info = HasCharacterList;
        }

        public CharacterManager.CharacterInfo FindCharacterAtID(int ID)
        {
            for (int i = 0; i < HasCharacterList.Count; i++)
            {
                if (HasCharacterList[i] == null)
                    continue;

                if (HasCharacterList[i].ID == ID)
                    return HasCharacterList[i];
            }

            return null;
        }


        public CharacterManager.CharacterInfo FindCharacterAtName(string Name)
        {
            for (int i = 0; i < HasCharacterList.Count; i++)
            {
                if (HasCharacterList[i] == null)
                    continue;

                if (HasCharacterList[i].CharacterName == Name)
                    return HasCharacterList[i];
            }

            return null;
        }
    }


    public string ObjectToJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    public T JsonToObject<T>(string jsonData)
    {
        return JsonUtility.FromJson<T>(jsonData);
    }

    public void CreateJsonFile(string createPath, string fileName, string jsonData)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", createPath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    public T LoadJsonFile<T>(string loadPath, string fileName)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", loadPath, fileName), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);

        return JsonUtility.FromJson<T>(jsonData);
    }
    
    public void FIleSave(SaveJsonFile saveFile)
    {
        string ID = saveFile.ID;
        string data = this.ObjectToJson(saveFile);
        this.CreateJsonFile(Application.dataPath, ID, data);
    }
   


    //사용예시
    void Start()
    {
        /*
        JTestClass jtc = new JTestClass(true);
        string jsonData = ObjectToJson(jtc);
        Debug.Log(jsonData);

        CreateJsonFile(Application.dataPath, "JTestClass", jsonData);
        

        var jtc2 = LoadJsonFile<JTestClass>(Application.dataPath, "JTestClass");
        jtc2.Print();
        */
        
    }

   
}
