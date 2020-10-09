using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Bibliotecas para escrever e apagar arquivos
using System;
using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Classe responsavel pelo gerenciamento de dados do jogador
/// </summary>
public class scr_SaveManager {

    public scr_Player_Stats playerStats = null;
    public bool hasLoaded = false;

    private DataSaver<scr_Player_Stats> dataSaver;

    private string savePath = "PlayerStatus.dat";

    public scr_SaveManager()
    {
        dataSaver = new DataSaver<scr_Player_Stats>(savePath,true,false);
    }

    /// <summary>
    /// Apaga arquivos atuais na memória
    /// </summary>
    /// <returns>Foi possivel apagar ou não</returns>
    public bool Delete()
    {
        if(dataSaver.SaveExists(0))
        {
            dataSaver.DeleteSave(0);
            return true;
        }
        return false;

        // if (File.Exists(Application.persistentDataPath + "/PlayerStatus.dat"))
        // {
        //     File.Delete(Application.persistentDataPath + "/PlayerStatus.dat");
        //     playerStats = new scr_Player_Stats();
        //     Save(playerStats);
        //     return true;
        // }
        // else
        //     return false;
    }

    /// <summary>
    /// Salva o stats dados em memória persistente
    /// </summary>
    /// <param name="newStats">Os stats a serem salvos na memória</param>
    /// <returns>Resultado da operação</returns>
    public bool Save(scr_Player_Stats newStats)
    {
        playerStats = newStats;
        dataSaver.SaveData(playerStats, 0);
        return true;
        // playerStats = newStats;
        // //BinaryFormatter bf = new BinaryFormatter();
        // //FileStream file = File.Create(Application.persistentDataPath + "/PlayerStatus.dat");


        // //bf.Serialize(file, playerStats);

        // //file.Close();

        // string dataAsJason = JsonUtility.ToJson(playerStats);

        // File.WriteAllText(Application.persistentDataPath + "/PlayerStatus.dat", dataAsJason);

        // return true;
    }

    /// <summary>
    /// Tenta carregar os arquivos do jogador da memória
    /// </summary>
    /// <returns>O perfil do jogador. Se não houver, retorna null.</returns>
    public scr_Player_Stats Load()
    {
        try
        {
            if(dataSaver.SaveExists(0))
                playerStats = dataSaver.LoadData(0);
            else
                playerStats = null;
        }
        catch
        {
            playerStats = null;
        }
        return playerStats;

        // //Verifica se o arquivo existe
        // if (File.Exists(Application.persistentDataPath + "/PlayerStatus.dat"))
        // {

        //     //BinaryFormatter bf = new BinaryFormatter();
        //     //FileStream file = File.Open(Application.persistentDataPath + "/PlayerStatus.dat", FileMode.Open);
        //     string dataAsJason = File.ReadAllText(Application.persistentDataPath + "/PlayerStatus.dat");
        //     playerStats =  JsonUtility.FromJson<scr_Player_Stats>(dataAsJason);


        //     //playerStats = (scr_Player_Stats)bf.Deserialize(file);
        //     //file.Close();
        //     hasLoaded = true;
        //     return playerStats;
        // }
        // else
        //     return null;
    }

    /// <summary>
	/// Função que retorna se possui um save game ou não
	/// </summary>
	/// <returns>Se o jogo posssui save ou não</returns>
    public bool hasSaveGame()
    {
        if (Load() == null || playerStats.savePointScene.Equals("null"))
            return false;
        else
            return true;
    }

}
