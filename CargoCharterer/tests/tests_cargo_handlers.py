import unittest

from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker

from app.application.models import Base
from config import config



class TestsCargoHanders(unittest.TestCase):

    def setUp(self):
        db_uri = config['development'].SQLALCHEMY_DATABASE_URI
        self.engine = create_engine(db_uri)
        self.uow = sessionmaker(bind=self.engine)
        Base.metadata.create_all(self.engine)

    def tearDown(self):
        Base.metadata.drop_all(self.engine)


    def test_add_cargo_command(self):





if __name__ == '__main__':
    unittest.main()
