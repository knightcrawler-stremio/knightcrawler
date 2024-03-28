import inspect
import logging
import os
import sys
from typing import Any

import structlog

LOGGER = os.getenv("LOGGER") or "console"

root_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

LOG_LEVEL = os.getenv("LOG_LEVEL", "info")
logging.basicConfig(
    format="%(message)s",
    stream=sys.stderr,
    level=LOG_LEVEL.upper(),
)


def add_code_info(_: logging.Logger, __: str, event_dict: Any) -> dict[str, Any]:
    frame = inspect.currentframe().f_back.f_back.f_back.f_back.f_back  # type: ignore
    event_dict["code_func"] = frame.f_code.co_name  # type: ignore
    # fname: str = frame.f_code.co_filename.replace(root_dir, "").lstrip("/")  # type: ignore
    event_dict["code_line"] = frame.f_lineno  # type: ignore
    # event_dict["code_file"] = f"{fname}:{frame.f_lineno}"  # type: ignore
    return event_dict


structlog.configure(
    processors=[
        structlog.stdlib.filter_by_level,
        structlog.contextvars.merge_contextvars,
        structlog.stdlib.add_log_level,
        structlog.stdlib.add_logger_name,
        add_code_info,
        structlog.processors.TimeStamper(fmt="iso"),
        structlog.processors.EventRenamer(to="msg"),
        structlog.processors.StackInfoRenderer(),
        structlog.processors.format_exc_info,
        (
            structlog.processors.JSONRenderer()
            if LOGGER == "json"
            else structlog.dev.ConsoleRenderer(event_key="msg")
        ),
    ],
    context_class=dict,
    logger_factory=structlog.stdlib.LoggerFactory(),
    wrapper_class=structlog.stdlib.BoundLogger,
    cache_logger_on_first_use=True,
)


def init():
    # the work is done on load
    return None
