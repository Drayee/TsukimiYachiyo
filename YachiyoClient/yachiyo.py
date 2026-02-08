import json, requests,tkinter,threading

config = dict()
text = None

def init():
    global config
    try:
        with open("config.json", "r") as f:
            config = json.load(f)
    except FileNotFoundError:
        config = {"url" : "http://1bo19870064ac.vicp.fun/api/v1/ai/chat"}
        with open("config.json", "w") as f:
            json.dump(config, f)


class Yachiyo(tkinter.Tk):
    def __init__(self):
        global text
        super().__init__()
        self.title("Yachiyo")
        self.geometry("300x300")
        self.resizable(width=False, height=True)
        self.entry = None

        text = tkinter.StringVar()

        self.build()

    def build(self):
        self.frame=tkinter.Frame(self)

        tkinter.Label(self.frame, text="Yachiyo").pack(expand=True, fill=tkinter.BOTH)
        tkinter.Label(self.frame, text="Enter your message:").pack(expand=True, fill=tkinter.BOTH)
        self.entry=tkinter.Entry(self.frame)
        self.entry.pack(expand=True, fill=tkinter.BOTH)
        tkinter.Button(self.frame, text="Send", command=lambda : self.send()).pack(expand=True, fill=tkinter.BOTH)
        tkinter.Label(self.frame, textvariable=text).pack(expand=True, fill=tkinter.BOTH)
        self.frame.pack()


    def send(self):
        message=self.entry.get()
        print(message)
        thread = threading.Thread(target=self.send_message, args=(message,))
        thread.start()

    @staticmethod
    def send_message(message):
        global config, text
        url = config["url"]
        text.set(requests.post(url, data=message).text)

if __name__ == "__main__":
    init()
    youtube = Yachiyo()
    youtube.mainloop()
