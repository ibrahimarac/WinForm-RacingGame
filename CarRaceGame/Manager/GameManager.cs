using CarRaceGame.Properties;
using CarRaceGame.UserControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using System.IO;
using System.Windows.Forms;

namespace CarRaceGame.Manager
{
    /*
     * Aşağıdaki Struct yardımıyla sahneye eklenecek nesnenin yukarıdan
     * aşağıya doğru hareket ederken boyunun orantılı bir şekilde artmasını
     * sağlamaya çalışıyorum. Böylece nesnenin yaklaştığı izlenimini vermek
     * istiyorum.
    */
    public struct ObjectSizeLimits
    {
        public int MinObjectWidth { get; set; }
        public int MaxObjectWidth { get; set; }
        public int MinObjectHeight { get; set; }
        public int MaxObjectHeight { get; set; }
    }

    public class GameManager
    {
        //Sahnedeki oyun nesnelerini saklamak için aşağıdaki liste kullanılacak.
        private List<IGameObject> gameObjects = new List<IGameObject>();
        //Formdaki elemanlara ulaşmak için formun adresi burada saklanıyor.
        Form1 frmParent;
        //Nesnelerin başlangıç ve bitiş boyutlarını aşağıdaki koleksiyonla tutuyoruz.
        Dictionary<ObjectType, ObjectSizeLimits> sizeLimits;

        public GameManager(Form1 frmParent)
        {
            this.frmParent = frmParent;
            /*
                Nesnelerin sahneye eklendiğinde ve sahnenin sonunda sahip olacağı
                boyutları nesne türüne göre bir sözlükte saklıyoruz.
            */
            sizeLimits = new Dictionary<ObjectType, ObjectSizeLimits>();
            sizeLimits.Add(ObjectType.Car, new ObjectSizeLimits
            {
                MaxObjectWidth = 90,
                MaxObjectHeight = 170,
                MinObjectWidth = 60,
                MinObjectHeight = 110
            });
            sizeLimits.Add(ObjectType.Star, new ObjectSizeLimits
            {
                MaxObjectWidth = 70,
                MaxObjectHeight = 70,
                MinObjectWidth = 50,
                MinObjectHeight = 50
            });
            sizeLimits.Add(ObjectType.Obstacle, new ObjectSizeLimits
            {
                MaxObjectWidth = 110,
                MaxObjectHeight = 110,
                MinObjectWidth = 70,
                MinObjectHeight = 70
            });
        }

        /*
            Ses çalmak için NAudio isimli nuget paketini ekliyorum. 
            Bu kütüphane yardımıyla mp3 dosyalarını C# ile çalabiliyoruz.
        */
        void PlayCrashSound()
        {
            WaveStream soundStream = new Mp3FileReader(new MemoryStream(Resources.crash));
            WaveOut soundPlayer = new WaveOut();
            soundPlayer.Init(soundStream);
            soundPlayer.Play();
        }

        void PlayClickSound()
        {
            WaveStream soundStream = new Mp3FileReader(new MemoryStream(Resources.star_pick));
            WaveOut soundPlayer = new WaveOut();
            soundPlayer.Init(soundStream);
            soundPlayer.Play();
        }

        Bitmap GetRandomResource()
        {
            //Resource dosyaları içerisinden rasgele bir dosya seçelim.
            Bitmap bmpCar = null;
            int resNumber = new Random().Next(1, 4);
            string resName = string.Format("car_{0}", resNumber);
            bmpCar = (Bitmap)Resources.ResourceManager.GetObject(resName);
            return bmpCar;
        }

        void CreateGameObject(ObjectType objType)
        {
            /*
                Eğer daha önce sahnede bir araç varsa yeni eklenecek araçla 
                bu araç arasında bir araç boyu mesafe eklemeye çalışıyoruz.
                Yeni eklenecek aracı bu mesafe istenen seviyeye ulaşana kadar
                eklemiyoruz.
            */
            if (gameObjects.Count != 0)
            {
                if (gameObjects[gameObjects.Count - 1].CurrentPosition <= sizeLimits[ObjectType.Car].MaxObjectHeight * 2)
                {
                    return;
                }
            }

            GameObject obj = new GameObject();
            obj.Type = objType;

            switch (objType)
            {
                case ObjectType.Car:
                    obj.SetObjectImage(GetRandomResource());
                    break;
                case ObjectType.Star:
                    obj.BackgroundImage = Resources.star;
                    break;
                case ObjectType.Obstacle:
                    obj.BackgroundImage = Resources.bush;
                    break;
            }
            obj.Width = sizeLimits[objType].MinObjectWidth;
            obj.Height = sizeLimits[objType].MinObjectHeight;                      

            /*
              Aracı sahnenin daha üstünden eklemek için 
              yüksekliği kadar yukarıdan başlatıyorum. Böylece oyunu
              oynayan kişi aracın ileriden geldiğini düşünecek.
            */
            obj.Top = -obj.Height;

            //Oyun alanına eklenecek nesneyi sol veya sağ şeride rasgele ekleyelim.
            obj.Left = (new Random().Next(0, 1000) % 2 == 0) ? 45 : 205;

            //Nesneyi hem forma hemde listeye ekleyelim.
            frmParent.Controls.Add(obj);
            gameObjects.Add(obj);
        }

        public void AddObjectToGameArea(ObjectType objType)
        {
            CreateGameObject(objType);
        }

        void RemoveGameObject(IGameObject gameObj)
        {
            frmParent.Controls.Remove(gameObj as GameObject);
            gameObjects.Remove(gameObj);
        }


        public void UpdateCars(int speed)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                IGameObject obj = gameObjects[i];
                if (obj.IsOutOfGameArea)
                {
                    RemoveGameObject(obj);
                }
                else
                {
                    /*
                        Aracın konumunu değiştirelim. Böylece form üzerinde aracı 
                        ileri doğru hareket ettirmiş oluyoruz.
                    */
                    obj.SetPosition(speed);
                    /*
                         Sahnedeki aracın yeni boyutunu hesaplarken nesnenin ilk boyutu
                         ile son boyutunu form üzerindeki konumu ile oranlıyorum.
                         Böylece olması gereken boyutu tespit edebiliyorum.
                    */
                    int minWidth = sizeLimits[obj.Type].MinObjectWidth;
                    int minHeight = sizeLimits[obj.Type].MinObjectHeight;
                    int maxWidth = sizeLimits[obj.Type].MaxObjectWidth;
                    int maxHeight = sizeLimits[obj.Type].MaxObjectHeight;
                    int newWidth = minWidth + 
                        (obj.CurrentPosition * (maxWidth - minWidth)) / frmParent.Height;
                    int newHeight = minHeight + 
                        (obj.CurrentPosition * (maxHeight - minHeight)) / frmParent.Height;
                    obj.SetObjectSize(newWidth, newHeight);
                }
            }

            //Herhangi bir nesne ile kesişim var mı onu kontrol edelim.

            IGameObject objHit = HitTest();

            //Uygun sesi çalıştıralım
            if (objHit != null)
            {
                switch (objHit.Type)
                {
                    case ObjectType.Car:
                        PlayCrashSound();
                        break;
                    case ObjectType.Obstacle:
                        PlayCrashSound();
                        break;
                    case ObjectType.Star:
                        PlayClickSound();
                        break;
                }
                RemoveGameObject(objHit);
            }
        }

        IGameObject HitTest()
        {
            IGameObject obj = null;
            /*
                Sahnedeki bizim kontrol ettiğimiz oyuncunun etrafını çevreleyen dikdörtgeni
                belirliyoruz. Karşıdan gelen nesnenin sol üst köşe noktasının bu dikdörtgenin
                içerisinde olup olmadığını kontrol ediyoruz. Eğer içerisinde ise o zaman
                karşıdan gelen nesneye çapmışız demektir.
            */
            var playerRectangle = new Rectangle(new Point(frmParent.Player.Left, frmParent.Player.Top), frmParent.Player.Size);
            
            /*
                Sahnede yer alan tüm nesnelerin player ile kesiştiğini test ediyoruz.   
            */
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (playerRectangle.Contains(gameObjects[i].ObjectRectangle.Location))
                {
                    /*
                        Eğer player'ın karşıdan gelen nesne ile kesiştiğini tespit ettiysek
                        çarpılan nesneyi obj içerisinde saklıyoruz.
                    */
                    obj = gameObjects[i];
                    break;
                }
            }
            return obj;
        }
    }
}



