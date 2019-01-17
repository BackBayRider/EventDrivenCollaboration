from enum import Enum, unique

from sqlalchemy import Column, Integer, String
from sqlalchemy.dialects.postgresql.base import UUID
from sqlalchemy.ext.declarative import declarative_base
from uuid import uuid4

Base = declarative_base()

@unique
class CargoType(Enum):
    CT_BREAK_BULK=1,
    CT_BULK=2,
    CT_NEO_BULK=3,
    CT_CONTAINER=4,
    CT_HEAVY_LIFT=5


class Cargo(Base):
    __tablename__ = 'cargo'

    id = Column(UUID(as_uuid=True), primary_key=True, default=uuid4)
    size = Column(Integer)
    type = Column(String)

    def __init__(self, id: UUID=None, size:int=0, type:str=None):
        self.id = id
        self.size = size
        self.type = CargoType[type]


    def __repr__(self):
        return "<Cargo(id='%s', type='%s', size='%d' >" % (self.id, self.type, self.size)


class Charterer(Base):
    __tablename__ = 'charterer'

    id = Column(UUID(as_uuid=True), primary_key=True, default=uuid4)
    name = Column(String)

    def __repr__(self):
        return "<Charterer(id='%s', name='%s' >" % (self.id, self.name)
