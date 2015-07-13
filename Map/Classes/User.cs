using Map.Resources;
using System;
using System.Runtime.Serialization;
using System.Windows;

namespace Map
{
    [DataContract]
    public class User
    {
        [DataMember]
        public  int age;

        [DataMember]
        public  double weight;

        [DataMember]
        public  double grade;

        [DataMember]
        public bool? gender;
        
        public User()
        {
            grade = 0.02;
            weight = 60;
        }
        public User(int a, double w, double g, bool? s)
        {
            if (g > 100 || g < 0)
            {
                MessageBox.Show(AppResources.InvalidUserInfo,"Warning",MessageBoxButton.OK);
                return;
            }
            age = a;
            weight = w;
            grade = g;
            gender = s;
        }

       
    }
}
