from UUID import uuid


from brightside.handler import Command

class NewCargoCommand(Command):
    def ___init__(self, id: UUID=None, type: str=None, size: int=0):
        self.id = id
        self.Type =
        self.Size = size
