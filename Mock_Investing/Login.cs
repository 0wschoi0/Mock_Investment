using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
using Google.Cloud.Firestore;
using Firebase.Auth;

namespace Mock_Investing
{
    public partial class Login : Form
    {
        public string loginID = "";
        FirestoreDb db;
        FirebaseConfig config = new FirebaseConfig("AIzaSyDBwwgGtnnUruuoORjTiVLEvsI_0e87BUk");

        public Login()
        {
            InitializeComponent();
            string path = AppDomain.CurrentDomain.BaseDirectory + @"mock-af23d-firebase-adminsdk-j7rje-256b87a451.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            db = FirestoreDb.Create("mock-af23d");
            txtEmail.Select();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Focus();
            btnLogin.Enabled = false;
            if (txtPassword.TextLength < 6)
            {
                MessageBox.Show("비밀번호는 최소 6자 이상이어야 합니다.");
                btnLogin.Enabled = true;
                txtPassword.Focus();
                return;
            }
            bool t = true;
            LoginCheck(t);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private async void LoginCheck(bool t)
        {

            var client = new FirebaseAuthProvider(config);

            try
            {
                var userCredential = await client.SignInWithEmailAndPasswordAsync(txtEmail.Text, txtPassword.Text);
                loginID = userCredential.User.LocalId;
                Console.WriteLine(loginID);
                new Dashboard(loginID).Show();
                this.Close();
            }
            catch (Firebase.Auth.FirebaseAuthException)
            {
                MessageBox.Show("이메일과 비밀번호를 확인하십시오.");
                txtPassword.ResetText();
                txtPassword.Focus();
                t = false;
                btnLogin.Enabled = true;
            }
        }
        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }
        private void btnSIgnUp_Click(object sender, EventArgs e)
        {
            sideTabControl.SelectedIndex = 1;
        }
        private void btnSignUpNew_Click(object sender, EventArgs e)
        {
            if (txtPasswordNew.TextLength < 6)
            {
                MessageBox.Show("비밀번호는 최소 6자 이상이어야 합니다.");
                return;
            }
            bool t = true;
            SignUpCheck(t);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            sideTabControl.SelectedIndex = 0;
        }
        private async void SignUpCheck(bool t)
        {
           var client = new FirebaseAuthProvider(config);
           try
           {
                var userCredential = await client.CreateUserWithEmailAndPasswordAsync(txtEmailNew.Text, txtPasswordNew.Text, txtNameNew.Text);
                MessageBox.Show("환영합니다! \n 가상화폐 모의투자를 이용해주셔서 감사합니다.");
                CollectionReference collection = db.Collection(userCredential.User.LocalId);
                DocumentReference document = collection.Document("Status");
                Dictionary<string, object> docData = new Dictionary<string, object>
                {
                    { "Name", txtNameNew.Text },
                    { "Asset", 30000000 },
                    { "Wallet", 30000000 },
                    { "CoinNumber", 0 },
                };
                DocumentReference document2 = collection.Document("Coins");
                Dictionary<string, object> coins = new Dictionary<string, object>()
                {

                };
                Dictionary<string, object> coinOwned = new Dictionary<string, object>(){ };
                coins.Add("CoinCurrent", coinOwned);
                await document.SetAsync(docData);
                await document2.SetAsync(coins);

                DocumentReference rankDoc = db.Collection("Ranking").Document("Top");
                Dictionary<string, object> data = new Dictionary<string, object>()
                {
                    {"UID."+txtNameNew.Text, userCredential.User.LocalId}
                };

                DocumentSnapshot snapshot = await rankDoc.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    await rankDoc.UpdateAsync(data);
                }

                sideTabControl.SelectedIndex = 0;
            }
           catch (FirebaseAuthException)
           {
               MessageBox.Show("이미 존재하는 이메일이거나 이메일 형식이 아닙니다 ");
               t = false;
           }
            finally
            {
                txtEmailNew.ResetText();
                txtPasswordNew.ResetText();
                txtNameNew.ResetText();
            }
        }

        private void chkPassword_CheckStateChanged(object sender, EventArgs e)
        {
            if (chkPassword.Checked == false){
                txtPassword.PasswordChar = '*';
            }
            else if (chkPassword.Checked == true)
            {
                txtPassword.PasswordChar = '\0';
            }
        }

        private void chkPasswordNew_CheckStateChanged(object sender, EventArgs e)
        {
            if (chkPasswordNew.Checked == false)
            {
                txtPasswordNew.PasswordChar = '*';
            }
            else if (chkPasswordNew.Checked == true)
            {
                txtPasswordNew.PasswordChar = '\0';
            }
        }
    }

     
}
